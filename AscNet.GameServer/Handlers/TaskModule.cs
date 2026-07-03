using AscNet.Common.Database;
using AscNet.Common.MsgPack;
using AscNet.Common.Util;
using AscNet.Table.V2.share.task;
using AscNet.Table.V2.share.reward;
using MessagePack;
using LoginTask = AscNet.Common.MsgPack.NotifyTaskData.NotifyTaskDataTaskData.NotifyTaskDataTaskDataTask;
using LoginTaskSchedule = AscNet.Common.MsgPack.NotifyTaskData.NotifyTaskDataTaskData.NotifyTaskDataTaskDataTask.NotifyTaskDataTaskDataTaskSchedule;
using SyncTask = AscNet.Common.MsgPack.NotifyTask.NotifyTaskTasks.NotifyTaskTasksTask;
using SyncTaskSchedule = AscNet.Common.MsgPack.NotifyTask.NotifyTaskTasks.NotifyTaskTasksTask.NotifyTaskTasksTaskSchedule;

namespace AscNet.GameServer.Handlers
{

    #region MsgPackScheme
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [MessagePackObject(true)]
    public class GetCourseRewardRequest
    {
        public int StageId;
    }
    
    [MessagePackObject(true)]
    public class GetCourseRewardResponse
    {
        public int Code;
        public List<RewardGoods> RewardGoodsList { get; set; } = new();
    }

    [MessagePackObject(true)]
    public class FinishTaskRequest
    {
        public int TaskId { get; set; }
    }

    [MessagePackObject(true)]
    public class FinishTaskResponse
    {
        public int Code { get; set; }
        public List<RewardGoods> RewardGoodsList { get; set; } = new();
    }

    [MessagePackObject(true)]
    public class FinishMultiTaskRequest
    {
        public List<int> TaskIds { get; set; } = new();
    }

    [MessagePackObject(true)]
    public class FinishMultiTaskResponse
    {
        public int Code { get; set; }
        public List<RewardGoods> RewardGoodsList { get; set; } = new();
        public List<int> NotDealTaskIds { get; set; } = new();
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    #endregion

    internal class TaskModule
    {
        [RequestPacketHandler("DoClientTaskEventRequest")]
        public static void DoClientTaskEventRequestHandler(Session session, Packet.Request packet)
        {
            session.SendResponse(new DoClientTaskEventResponse(), packet.Id);
        }

        [RequestPacketHandler("FinishTaskRequest")]
        public static void FinishTaskRequestHandler(Session session, Packet.Request packet)
        {
            FinishTaskRequest request = MessagePackSerializer.Deserialize<FinishTaskRequest>(packet.Content);
            FinishTaskResponse response = ClaimStoryTaskReward(session, request.TaskId, pushSync: true);
            session.SendResponse(response, packet.Id);
        }

        [RequestPacketHandler("FinishMultiTaskRequest")]
        public static void FinishMultiTaskRequestHandler(Session session, Packet.Request packet)
        {
            FinishMultiTaskRequest request = MessagePackSerializer.Deserialize<FinishMultiTaskRequest>(packet.Content);
            FinishMultiTaskResponse response = new()
            {
                Code = 0
            };

            foreach (int taskId in request.TaskIds.Distinct())
            {
                FinishTaskResponse taskResponse = ClaimStoryTaskReward(session, taskId, pushSync: false);
                if (taskResponse.Code == 0)
                {
                    response.RewardGoodsList.AddRange(taskResponse.RewardGoodsList);
                }
                else
                {
                    response.NotDealTaskIds.Add(taskId);
                }
            }

            SendStoryTaskSync(session);
            session.SendResponse(response, packet.Id);
        }

        [RequestPacketHandler("GetCourseRewardRequest")]
        public static void GetCourseRewardRequestHandler(Session session, Packet.Request packet)
        {
            var request = MessagePackSerializer.Deserialize<GetCourseRewardRequest>(packet.Content);
            GetCourseRewardResponse response = ClaimCourseReward(session, request.StageId);
            session.SendResponse(response, packet.Id);
        }

        private static GetCourseRewardResponse ClaimCourseReward(Session session, int stageId)
        {
            if (!session.stage.Stages.TryGetValue(stageId, out StageDatum? stageData) || !stageData.Passed)
            {
                return new GetCourseRewardResponse { Code = 20026013 };
            }

            CourseTable? courseTable = TableReaderV2.Parse<CourseTable>().FirstOrDefault(x => x.StageId == stageId);
            if (courseTable is null || courseTable.RewardId <= 0)
            {
                return new GetCourseRewardResponse { Code = 20026013 };
            }

            List<RewardGoodsTable> rewardGoods = GetRewardGoods(courseTable.RewardId);
            if (rewardGoods.Count == 0)
            {
                return new GetCourseRewardResponse { Code = 20026013 };
            }

            if (!session.stage.AddCourse((uint)stageId))
            {
                return new GetCourseRewardResponse { Code = 20026014 };
            }

            List<RewardGoods> rewardGoodsList = RewardHandler.GiveRewards(rewardGoods, session);
            session.inventory.Save();
            session.character.Save();
            session.stage.Save();

            return new GetCourseRewardResponse
            {
                Code = 0,
                RewardGoodsList = rewardGoodsList
            };
        }

        public static List<LoginTask> BuildStoryTaskData(Session session)
        {
            return BuildStoryTaskProgress(session)
                .Select(ToLoginTask)
                .ToList();
        }

        public static void SendStoryTaskSync(Session session)
        {
            session.SendPush(new NotifyTask
            {
                Tasks = new()
                {
                    Tasks = BuildStoryTaskProgress(session)
                        .Select(ToSyncTask)
                        .ToList()
                }
            });
        }

        private static FinishTaskResponse ClaimStoryTaskReward(Session session, int taskId, bool pushSync)
        {
            StoryTaskTable? task = TableReaderV2.Parse<StoryTaskTable>().FirstOrDefault(x => x.Id == taskId);
            if (task is null)
            {
                return new FinishTaskResponse { Code = 20026005 };
            }

            if (session.stage.FinishedTasks.Contains(taskId))
            {
                return new FinishTaskResponse { Code = 20026006 };
            }

            StoryTaskProgress? progress = BuildStoryTaskProgress(session).FirstOrDefault(x => x.TaskId == taskId);
            if (progress is null || progress.State != TaskStateAchieved)
            {
                return new FinishTaskResponse { Code = 20026007 };
            }

            List<RewardGoodsTable> rewardGoods = GetRewardGoods(task.RewardId);
            if (rewardGoods.Count == 0)
            {
                return new FinishTaskResponse { Code = 20026003 };
            }

            if (!session.stage.AddFinishedTask(taskId))
            {
                return new FinishTaskResponse { Code = 20026006 };
            }

            List<RewardGoods> rewardGoodsList = RewardHandler.GiveRewards(rewardGoods, session);
            session.inventory.Save();
            session.character.Save();
            session.stage.Save();

            if (pushSync)
            {
                SendStoryTaskSync(session);
            }

            return new FinishTaskResponse
            {
                Code = 0,
                RewardGoodsList = rewardGoodsList
            };
        }

        private static List<StoryTaskProgress> BuildStoryTaskProgress(Session session)
        {
            Dictionary<int, StoryTaskTable> tasks = TableReaderV2.Parse<StoryTaskTable>().ToDictionary(x => x.Id);
            Dictionary<int, StoryTaskConditionTable> conditions = TableReaderV2.Parse<StoryTaskConditionTable>().ToDictionary(x => x.Id);
            Dictionary<int, int> progressCache = new();

            int GetProgress(StoryTaskTable task)
            {
                if (session.stage.FinishedTasks.Contains(task.Id))
                {
                    return task.Result;
                }

                if (progressCache.TryGetValue(task.Id, out int cachedProgress))
                {
                    return cachedProgress;
                }

                int conditionId = task.Condition;
                int progress = 0;
                if (conditionId != 0 && conditions.TryGetValue(conditionId, out StoryTaskConditionTable? condition))
                {
                    progress = EvaluateStoryTaskCondition(session, condition, tasks, GetProgress);
                }

                progress = Math.Min(progress, task.Result);
                progressCache[task.Id] = progress;
                return progress;
            }

            return tasks.Values
                .OrderByDescending(x => x.Priority)
                .Select(task =>
                {
                    int progress = GetProgress(task);
                    int state = session.stage.FinishedTasks.Contains(task.Id)
                        ? TaskStateFinish
                        : progress >= task.Result ? TaskStateAchieved : TaskStateActive;
                    return new StoryTaskProgress(task.Id, task.Condition, progress, state);
                })
                .ToList();
        }

        private static int EvaluateStoryTaskCondition(Session session, StoryTaskConditionTable condition, IReadOnlyDictionary<int, StoryTaskTable> tasks, Func<StoryTaskTable, int> getProgress)
        {
            return condition.Type switch
            {
                10202 => HasCompletedPrologue(session) ? 1 : 0,
                15201 or 15220 or 15222 => HasPassedEveryStageParam(session, condition) ? 1 : 0,
                15219 => HasPassedEveryStageParam(session, condition) ? 1 : 0,
                17203 => CountCompletedChildTasks(condition, tasks, getProgress),
                _ => 0
            };
        }

        private static bool HasCompletedPrologue(Session session)
        {
            return session.stage.Stages.Values.Any(x => x.Passed);
        }

        private static bool HasPassedEveryStageParam(Session session, StoryTaskConditionTable condition)
        {
            List<int> stageIds = condition.Params.Where(x => x >= 10_000_000).ToList();
            return stageIds.Count > 0 && stageIds.All(stageId => session.stage.Stages.TryGetValue(stageId, out StageDatum? stageData) && stageData.Passed);
        }

        private static int CountCompletedChildTasks(StoryTaskConditionTable condition, IReadOnlyDictionary<int, StoryTaskTable> tasks, Func<StoryTaskTable, int> getProgress)
        {
            return condition.Params
                .Skip(1)
                .Where(tasks.ContainsKey)
                .Count(taskId =>
                {
                    StoryTaskTable task = tasks[taskId];
                    return getProgress(task) >= task.Result;
                });
        }

        private static LoginTask ToLoginTask(StoryTaskProgress progress)
        {
            return new LoginTask
            {
                Id = (uint)progress.TaskId,
                State = progress.State,
                RecordTime = 0,
                ActivityId = 0,
                Schedule =
                [
                    new LoginTaskSchedule
                    {
                        Id = (uint)progress.ConditionId,
                        Value = progress.Value
                    }
                ]
            };
        }

        private static SyncTask ToSyncTask(StoryTaskProgress progress)
        {
            return new SyncTask
            {
                Id = (uint)progress.TaskId,
                State = progress.State,
                RecordTime = 0,
                ActivityId = 0,
                Schedule =
                [
                    new SyncTaskSchedule
                    {
                        Id = (uint)progress.ConditionId,
                        Value = progress.Value
                    }
                ]
            };
        }

        private static List<RewardGoodsTable> GetRewardGoods(int rewardId)
        {
            RewardTable? rewardTable = TableReaderV2.Parse<RewardTable>().FirstOrDefault(x => x.Id == rewardId);
            if (rewardTable is null)
            {
                return [];
            }

            HashSet<int> subIds = rewardTable.SubIds.ToHashSet();
            if (subIds.Count == 0)
            {
                return [];
            }

            return TableReaderV2.Parse<RewardGoodsTable>()
                .Where(x => subIds.Contains(x.Id))
                .ToList();
        }

        private const int TaskStateActive = 1;
        private const int TaskStateAchieved = 3;
        private const int TaskStateFinish = 4;

        private sealed record StoryTaskProgress(int TaskId, int ConditionId, int Value, int State);

    }
}
