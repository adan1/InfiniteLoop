using AscNet.Common.MsgPack;
using AscNet.Common.Util;
using AscNet.GameServer;
using AscNet.GameServer.Handlers;
using AscNet.SDKServer.Models;
using AscNet.Table.V2.share.guide;
using AscNet.Table.V2.share.fuben;
using AscNet.Table.V2.share.fuben.mainline;
using AscNet.Table.V2.share.task;
using AscNet.Table.V2.share.reward;
using AscNet.Table.V2.share.character;
using MessagePack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using LoginTask = AscNet.Common.MsgPack.NotifyTaskData.NotifyTaskDataTaskData.NotifyTaskDataTaskDataTask;

namespace AscNet.Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                UseResourceWorkingDirectory();
                if (args.Contains("--notify-login-compat-only"))
                {
                    ValidateNotifyLoginCurrentClientCompatibilityShape();
                    return;
                }

                if (args.Contains("--stage-bookmark-compat-only"))
                {
                    ValidateStageBookmarkCompatibilityShape();
                    return;
                }

                if (args.Contains("--mainline2-exhibition-compat-only"))
                {
                    ValidateMainLine2UpdateExhibitionChapterCompatibility();
                    return;
                }

                if (args.Contains("--mainline-treasure-reward-compat-only"))
                {
                    ValidateMainLineTreasureRewardCompatibility();
                    return;
                }

                if (args.Contains("--boss-single-login-compat-only"))
                {
                    ValidateBossSingleLoginCompatibilityShape();
                    return;
                }

                if (args.Contains("--guide-table-compat-only"))
                {
                    ValidateCurrentClientGuideTableCompatibility();
                    return;
                }

                if (args.Contains("--player-cost-time-upload-compat-only"))
                {
                    ValidatePlayerCostTimeUploadCompatibility();
                    return;
                }

                if (args.Contains("--record-player-point-compat-only"))
                {
                    ValidateRecordPlayerPointCompatibility();
                    return;
                }

                if (args.Contains("--player-gender-compat-only"))
                {
                    ValidatePlayerGenderCompatibility();
                    return;
                }

                if (args.Contains("--board-mutual-push-compat-only"))
                {
                    ValidateBoardMutualClientPushCompatibility();
                    return;
                }

                if (args.Contains("--character-progression-persistence-compat-only"))
                {
                    ValidateCharacterProgressionPersistenceCompatibility();
                    return;
                }

                if (args.Contains("--exp-level-compat-only"))
                {
                    ValidateExpLevelCompatibility();
                    return;
                }

                if (args.Contains("--story-course-reward-compat-only"))
                {
                    ValidateStoryCourseRewardCompatibility();
                    return;
                }

                if (args.Contains("--pr2-quality-compat-only"))
                {
                    ValidatePr2QualityCompatibility();
                    return;
                }

                if (args.Contains("--current-client-notice-endpoints-only"))
                {
                    ValidateCurrentClientNoticeEndpoints().GetAwaiter().GetResult();
                    return;
                }

                _ = JsonConvert.DeserializeObject<NotifyLogin>(File.ReadAllText(ResourcePath("Data", "NotifyLogin.json")))!;
                _ = JsonConvert.DeserializeObject<NotifyTaskData>(File.ReadAllText(ResourcePath("Data", "NotifyTaskData.json")))!;
                ValidateNotifyLoginCurrentClientCompatibilityShape();
                ValidateStageBookmarkCompatibilityShape();
                ValidateMainLine2UpdateExhibitionChapterCompatibility();
                ValidateMainLineTreasureRewardCompatibility();
                ValidateBossSingleLoginCompatibilityShape();
                ValidateCurrentClientGuideTableCompatibility();
                ValidatePlayerCostTimeUploadCompatibility();
                ValidateRecordPlayerPointCompatibility();
                ValidateBoardMutualClientPushCompatibility();
                ValidatePlayerGenderCompatibility();
                ValidateCharacterProgressionPersistenceCompatibility();
                ValidateExpLevelCompatibility();
                ValidateStoryCourseRewardCompatibility();
                ValidatePr2QualityCompatibility();
                ValidateCurrentClientNoticeFixtures();
                ValidateCurrentClientNoticeEndpoints().GetAwaiter().GetResult();
                ValidateSteamClientConfig();
                ValidateKuroSdkCompatibilityEndpoints().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                Environment.ExitCode = 1;
            }
        }

        private static void ValidateNotifyLoginCurrentClientCompatibilityShape()
        {
            NotifyLogin login = new()
            {
                FubenMainLine2Data = new()
                {
                    StageDataList = [],
                    ChapterDataList = [],
                    TreasureData = [],
                    AchievementData = [],
                    EggData = [],
                    PassStageIds = []
                },
                FashionColorData = new()
                {
                    FasionColors = []
                }
            };

            NotifyLogin roundTrip = MessagePackSerializer.Deserialize<NotifyLogin>(MessagePackSerializer.Serialize(login));

            NotifyFashionColorData fashionColorData = roundTrip.FashionColorData
                ?? throw new InvalidDataException("NotifyLogin FashionColorData serialized as nil.");
            AssertEmptyList(fashionColorData.FasionColors, "NotifyLogin FashionColorData.FasionColors");

            FubenMainLine2Data fubenMainLine2Data = roundTrip.FubenMainLine2Data
                ?? throw new InvalidDataException("NotifyLogin FubenMainLine2Data serialized as nil.");
            AssertEmptyList(fubenMainLine2Data.StageDataList, "NotifyLogin FubenMainLine2Data.StageDataList");
            AssertEmptyList(fubenMainLine2Data.ChapterDataList, "NotifyLogin FubenMainLine2Data.ChapterDataList");
            AssertEmptyList(fubenMainLine2Data.TreasureData, "NotifyLogin FubenMainLine2Data.TreasureData");
            AssertEmptyList(fubenMainLine2Data.AchievementData, "NotifyLogin FubenMainLine2Data.AchievementData");
            AssertEmptyList(fubenMainLine2Data.EggData, "NotifyLogin FubenMainLine2Data.EggData");
            AssertEmptyList(fubenMainLine2Data.PassStageIds, "NotifyLogin FubenMainLine2Data.PassStageIds");
        }

        private static void ValidateStageBookmarkCompatibilityShape()
        {
            GetStageBookmarkResponse response = new();
            GetStageBookmarkResponse roundTrip = MessagePackSerializer.Deserialize<GetStageBookmarkResponse>(MessagePackSerializer.Serialize(response));

            AssertEqual(0, roundTrip.Code, "GetStageBookmarkResponse Code");
            AssertEmptyList(roundTrip.StageBookmarkList, "GetStageBookmarkResponse StageBookmarkList");
            AssertEmptyList(roundTrip.BookmarkList, "GetStageBookmarkResponse BookmarkList");
            ValidateRequestHandlerRegistration("GetStageBookmarkRequest");
        }

        private static void ValidateMainLine2UpdateExhibitionChapterCompatibility()
        {
            MainLine2UpdateExhibitionChapterResponse response = new()
            {
                Code = 0
            };
            MainLine2UpdateExhibitionChapterResponse roundTrip = MessagePackSerializer.Deserialize<MainLine2UpdateExhibitionChapterResponse>(
                MessagePackSerializer.Serialize(response));

            AssertEqual(0, roundTrip.Code, "MainLine2UpdateExhibitionChapterResponse Code");
            ValidateRequestHandlerRegistration("MainLine2UpdateExhibitionChapterRequest");
        }

        private static void ValidateMainLineTreasureRewardCompatibility()
        {
            const int treasureId = 1001002;
            const int requiredStars = 12;
            const int rewardId = 1001002;
            int[] expectedChapterStages =
            [
                10010101,
                10010102,
                10010103,
                10010104,
                10010201,
                10010202,
                10010203,
                10010204,
                10010301,
                10010302,
                10010303,
                10010304
            ];

            TreasureTable treasureTable = TableReaderV2.Parse<TreasureTable>().SingleOrDefault(treasure => treasure.TreasureId == treasureId)
                ?? throw new InvalidDataException($"TreasureTable: missing current mainline treasure {treasureId}.");
            AssertEqual(requiredStars, treasureTable.RequireStar, $"TreasureTable {treasureId} RequireStar");
            AssertEqual(rewardId, treasureTable.RewardId, $"TreasureTable {treasureId} RewardId");

            ChapterTable chapterTable = TableReaderV2.Parse<ChapterTable>().SingleOrDefault(chapter => chapter.TreasureId.Contains(treasureId))
                ?? throw new InvalidDataException($"ChapterTable: no current mainline chapter maps TreasureId {treasureId}.");
            AssertEqual(1001, chapterTable.ChapterId, $"ChapterTable TreasureId {treasureId} ChapterId");
            if (!chapterTable.StageId.Contains(expectedChapterStages[0]))
                throw new InvalidDataException($"ChapterTable {chapterTable.ChapterId}: expected TreasureId {treasureId} stages to include {expectedChapterStages[0]}.");
            if (!expectedChapterStages.SequenceEqual(chapterTable.StageId))
                throw new InvalidDataException($"ChapterTable {chapterTable.ChapterId}: expected TreasureId {treasureId} stages [{string.Join(", ", expectedChapterStages)}], got [{string.Join(", ", chapterTable.StageId)}].");

            MethodInfo tableParse = RequiredMethod(
                typeof(TableReaderV2),
                nameof(TableReaderV2.Parse),
                BindingFlags.Static | BindingFlags.Public);
            MethodInfo addTreasure = RequiredMethod(
                typeof(AscNet.Common.Database.Player),
                nameof(AscNet.Common.Database.Player.AddTreasure),
                BindingFlags.Instance | BindingFlags.Public,
                [typeof(int)]);
            MethodInfo playerSave = RequiredMethod(
                typeof(AscNet.Common.Database.Player),
                nameof(AscNet.Common.Database.Player.Save),
                BindingFlags.Instance | BindingFlags.Public);
            MethodInfo inventorySave = RequiredMethod(
                typeof(AscNet.Common.Database.Inventory),
                nameof(AscNet.Common.Database.Inventory.Save),
                BindingFlags.Instance | BindingFlags.Public);
            MethodInfo characterSave = RequiredMethod(
                typeof(AscNet.Common.Database.Character),
                nameof(AscNet.Common.Database.Character.Save),
                BindingFlags.Instance | BindingFlags.Public);
            Type rewardHandlerType = RequiredAscNetGameServerType("AscNet.GameServer.Handlers.RewardHandler");
            MethodInfo getRewardGoods = RequiredMethod(
                rewardHandlerType,
                "GetRewardGoods",
                BindingFlags.Static | BindingFlags.Public,
                [typeof(int)]);
            MethodInfo starsMarkGetter = RequiredMethod(
                typeof(StageDatum),
                $"get_{nameof(StageDatum.StarsMark)}",
                BindingFlags.Instance | BindingFlags.Public);

            MethodInfo treasureRewardHandler = GetRegisteredRequestHandlerMethod("ReceiveTreasureRewardRequest");
            AssertEqual("HandleReceiveTreasureRewardRequestHandler", treasureRewardHandler.Name, "ReceiveTreasureRewardRequest registered handler method");
            AssertMethodTransitivelyCallsGenericMethod(treasureRewardHandler, tableParse, typeof(TreasureTable), "ReceiveTreasureRewardRequestHandler treasure table lookup");
            AssertMethodTransitivelyCallsGenericMethod(treasureRewardHandler, tableParse, typeof(ChapterTable), "ReceiveTreasureRewardRequestHandler chapter table lookup");
            AssertMethodTransitivelyCalls(treasureRewardHandler, starsMarkGetter, "ReceiveTreasureRewardRequestHandler chapter star count source");
            AssertMethodTransitivelyCalls(treasureRewardHandler, addTreasure, "ReceiveTreasureRewardRequestHandler treasure claim marker");
            AssertMethodTransitivelyCalls(treasureRewardHandler, getRewardGoods, "ReceiveTreasureRewardRequestHandler reward goods resolution");
            AssertMethodTransitivelyCalls(treasureRewardHandler, playerSave, "ReceiveTreasureRewardRequestHandler player persistence");
            AssertMethodTransitivelyCalls(treasureRewardHandler, inventorySave, "ReceiveTreasureRewardRequestHandler inventory persistence");
            AssertMethodTransitivelyCalls(treasureRewardHandler, characterSave, "ReceiveTreasureRewardRequestHandler character persistence");
        }

        private static void ValidatePlayerCostTimeUploadCompatibility()
        {
            PlayerCostTimeUploadRequest request = new()
            {
                FunctionId = 1101,
                CostTime = 42
            };
            PlayerCostTimeUploadRequest requestRoundTrip = MessagePackSerializer.Deserialize<PlayerCostTimeUploadRequest>(
                MessagePackSerializer.Serialize(request));

            AssertEqual(1101, requestRoundTrip.FunctionId, "PlayerCostTimeUploadRequest FunctionId");
            AssertEqual(42, requestRoundTrip.CostTime, "PlayerCostTimeUploadRequest CostTime");

            PlayerCostTimeUploadResponse response = new()
            {
                Code = 0
            };
            PlayerCostTimeUploadResponse responseRoundTrip = MessagePackSerializer.Deserialize<PlayerCostTimeUploadResponse>(
                MessagePackSerializer.Serialize(response));

            AssertEqual(0, responseRoundTrip.Code, "PlayerCostTimeUploadResponse Code");
            ValidateRequestHandlerRegistration("PlayerCostTimeUploadRequest");
        }

        private static void ValidateRecordPlayerPointCompatibility()
        {
            RecordPlayerPointRequest request = new()
            {
                PointId = 31001,
                PointType = 2
            };
            RecordPlayerPointRequest requestRoundTrip = MessagePackSerializer.Deserialize<RecordPlayerPointRequest>(
                MessagePackSerializer.Serialize(request));

            AssertEqual(31001, requestRoundTrip.PointId, "RecordPlayerPointRequest PointId");
            AssertEqual(2, requestRoundTrip.PointType, "RecordPlayerPointRequest PointType");

            RecordPlayerPointResponse response = new()
            {
                Code = 0
            };
            RecordPlayerPointResponse responseRoundTrip = MessagePackSerializer.Deserialize<RecordPlayerPointResponse>(
                MessagePackSerializer.Serialize(response));

            AssertEqual(0, responseRoundTrip.Code, "RecordPlayerPointResponse Code");
            ValidateRequestHandlerRegistration("RecordPlayerPointRequest");
        }

        private static void ValidatePlayerGenderCompatibility()
        {
            const int selectedGender = 2;
            const int currentClientGender = 3;
            const long changeGenderTime = 1_720_000_123;
            const int firstSetupRewardCount = 50;

            PropertyInfo playerDataGender = typeof(PlayerData).GetProperty(nameof(PlayerData.Gender), BindingFlags.Instance | BindingFlags.Public)
                ?? throw new MissingMemberException(typeof(PlayerData).FullName, nameof(PlayerData.Gender));
            AssertEqual(typeof(long), playerDataGender.PropertyType, "PlayerData Gender type");
            MethodInfo playerDataGenderGetter = playerDataGender.GetMethod
                ?? throw new MissingMethodException(typeof(PlayerData).FullName, $"get_{nameof(PlayerData.Gender)}");
            MethodInfo playerDataGenderSetter = playerDataGender.SetMethod
                ?? throw new MissingMethodException(typeof(PlayerData).FullName, $"set_{nameof(PlayerData.Gender)}");

            PropertyInfo playerDataChangeGenderTime = typeof(PlayerData).GetProperty(nameof(PlayerData.ChangeGenderTime), BindingFlags.Instance | BindingFlags.Public)
                ?? throw new MissingMemberException(typeof(PlayerData).FullName, nameof(PlayerData.ChangeGenderTime));
            AssertEqual(typeof(long), playerDataChangeGenderTime.PropertyType, "PlayerData ChangeGenderTime type");
            MethodInfo playerDataChangeGenderTimeGetter = playerDataChangeGenderTime.GetMethod
                ?? throw new MissingMethodException(typeof(PlayerData).FullName, $"get_{nameof(PlayerData.ChangeGenderTime)}");
            MethodInfo playerDataChangeGenderTimeSetter = playerDataChangeGenderTime.SetMethod
                ?? throw new MissingMethodException(typeof(PlayerData).FullName, $"set_{nameof(PlayerData.ChangeGenderTime)}");

            PlayerData playerData = new()
            {
                Id = 9,
                Name = "GenderCompatibilityCommandant",
                Gender = selectedGender,
                ChangeGenderTime = changeGenderTime
            };
            PlayerData playerDataRoundTrip = MessagePackSerializer.Deserialize<PlayerData>(
                MessagePackSerializer.Serialize(playerData));
            AssertEqual((long)selectedGender, playerDataRoundTrip.Gender, "PlayerData Gender MessagePack round-trip");
            AssertEqual(changeGenderTime, playerDataRoundTrip.ChangeGenderTime, "PlayerData ChangeGenderTime MessagePack round-trip");

            MethodInfo createPlayer = RequiredMethod(
                typeof(AscNet.Common.Database.Player),
                "Create",
                BindingFlags.Static | BindingFlags.NonPublic,
                [typeof(long)]);
            AssertMethodTransitivelyCalls(createPlayer, playerDataGenderSetter, "Player.Create new-player gender availability");

            FieldInfo requestGender = typeof(ChangePlayerGenderRequest).GetField(nameof(ChangePlayerGenderRequest.Gender), BindingFlags.Instance | BindingFlags.Public)
                ?? throw new MissingFieldException(typeof(ChangePlayerGenderRequest).FullName, nameof(ChangePlayerGenderRequest.Gender));
            AssertEqual(typeof(int), requestGender.FieldType, "ChangePlayerGenderRequest Gender type");
            ChangePlayerGenderRequest request = new()
            {
                Gender = currentClientGender
            };
            ChangePlayerGenderRequest requestRoundTrip = MessagePackSerializer.Deserialize<ChangePlayerGenderRequest>(
                MessagePackSerializer.Serialize(request));
            AssertEqual(currentClientGender, requestRoundTrip.Gender, "ChangePlayerGenderRequest current-client gender MessagePack round-trip");

            FieldInfo responseCode = typeof(ChangePlayerGenderResponse).GetField(nameof(ChangePlayerGenderResponse.Code), BindingFlags.Instance | BindingFlags.Public)
                ?? throw new MissingFieldException(typeof(ChangePlayerGenderResponse).FullName, nameof(ChangePlayerGenderResponse.Code));
            FieldInfo responseGender = typeof(ChangePlayerGenderResponse).GetField(nameof(ChangePlayerGenderResponse.Gender), BindingFlags.Instance | BindingFlags.Public)
                ?? throw new MissingFieldException(typeof(ChangePlayerGenderResponse).FullName, nameof(ChangePlayerGenderResponse.Gender));
            FieldInfo responseChangeGenderTime = typeof(ChangePlayerGenderResponse).GetField(nameof(ChangePlayerGenderResponse.ChangeGenderTime), BindingFlags.Instance | BindingFlags.Public)
                ?? throw new MissingFieldException(typeof(ChangePlayerGenderResponse).FullName, nameof(ChangePlayerGenderResponse.ChangeGenderTime));
            FieldInfo responseNextCanChangeTime = typeof(ChangePlayerGenderResponse).GetField(nameof(ChangePlayerGenderResponse.NextCanChangeTime), BindingFlags.Instance | BindingFlags.Public)
                ?? throw new MissingFieldException(typeof(ChangePlayerGenderResponse).FullName, nameof(ChangePlayerGenderResponse.NextCanChangeTime));
            FieldInfo responsePlayerData = typeof(ChangePlayerGenderResponse).GetField(nameof(ChangePlayerGenderResponse.PlayerData), BindingFlags.Instance | BindingFlags.Public)
                ?? throw new MissingFieldException(typeof(ChangePlayerGenderResponse).FullName, nameof(ChangePlayerGenderResponse.PlayerData));
            FieldInfo responseRewardGoodsList = typeof(ChangePlayerGenderResponse).GetField(nameof(ChangePlayerGenderResponse.RewardGoodsList), BindingFlags.Instance | BindingFlags.Public)
                ?? throw new MissingFieldException(typeof(ChangePlayerGenderResponse).FullName, nameof(ChangePlayerGenderResponse.RewardGoodsList));
            AssertEqual(typeof(int), responseCode.FieldType, "ChangePlayerGenderResponse Code type");
            AssertEqual(typeof(long), responseGender.FieldType, "ChangePlayerGenderResponse Gender type");
            AssertEqual(typeof(long), responseChangeGenderTime.FieldType, "ChangePlayerGenderResponse ChangeGenderTime type");
            AssertEqual(typeof(long), responseNextCanChangeTime.FieldType, "ChangePlayerGenderResponse NextCanChangeTime type");
            AssertEqual(typeof(PlayerData), responsePlayerData.FieldType, "ChangePlayerGenderResponse PlayerData type");
            AssertEqual(typeof(List<RewardGoods>), responseRewardGoodsList.FieldType, "ChangePlayerGenderResponse RewardGoodsList type");
            ChangePlayerGenderResponse response = new()
            {
                Gender = selectedGender,
                ChangeGenderTime = changeGenderTime,
                NextCanChangeTime = changeGenderTime,
                PlayerData = playerData
            };
            response.RewardGoodsList.Add(new RewardGoods()
            {
                RewardType = (int)RewardType.Item,
                TemplateId = AscNet.Common.Database.Inventory.FreeGem,
                Count = firstSetupRewardCount
            });
            ChangePlayerGenderResponse responseRoundTrip = MessagePackSerializer.Deserialize<ChangePlayerGenderResponse>(
                MessagePackSerializer.Serialize(response));
            AssertEqual((long)selectedGender, responseRoundTrip.Gender, "ChangePlayerGenderResponse Gender MessagePack round-trip");
            AssertEqual(changeGenderTime, responseRoundTrip.ChangeGenderTime, "ChangePlayerGenderResponse ChangeGenderTime MessagePack round-trip");
            AssertEqual(changeGenderTime, responseRoundTrip.NextCanChangeTime, "ChangePlayerGenderResponse NextCanChangeTime MessagePack round-trip");
            if (responseRoundTrip.PlayerData is null)
                throw new InvalidDataException("ChangePlayerGenderResponse PlayerData MessagePack round-trip: expected player data.");
            AssertEqual((long)selectedGender, responseRoundTrip.PlayerData.Gender, "ChangePlayerGenderResponse PlayerData.Gender MessagePack round-trip");
            AssertEqual(changeGenderTime, responseRoundTrip.PlayerData.ChangeGenderTime, "ChangePlayerGenderResponse PlayerData.ChangeGenderTime MessagePack round-trip");
            AssertEqual(1, responseRoundTrip.RewardGoodsList.Count, "ChangePlayerGenderResponse RewardGoodsList MessagePack count");
            AssertEqual((int)RewardType.Item, responseRoundTrip.RewardGoodsList[0].RewardType, "ChangePlayerGenderResponse RewardGoodsList reward type");
            AssertEqual(AscNet.Common.Database.Inventory.FreeGem, responseRoundTrip.RewardGoodsList[0].TemplateId, "ChangePlayerGenderResponse RewardGoodsList Black Card item id");
            AssertEqual(firstSetupRewardCount, responseRoundTrip.RewardGoodsList[0].Count, "ChangePlayerGenderResponse RewardGoodsList Black Card count");

            FieldInfo notifyGender = typeof(NotifyPlayerGender).GetField(nameof(NotifyPlayerGender.Gender), BindingFlags.Instance | BindingFlags.Public)
                ?? throw new MissingFieldException(typeof(NotifyPlayerGender).FullName, nameof(NotifyPlayerGender.Gender));
            FieldInfo notifyChangeGenderTime = typeof(NotifyPlayerGender).GetField(nameof(NotifyPlayerGender.ChangeGenderTime), BindingFlags.Instance | BindingFlags.Public)
                ?? throw new MissingFieldException(typeof(NotifyPlayerGender).FullName, nameof(NotifyPlayerGender.ChangeGenderTime));
            AssertEqual(typeof(long), notifyGender.FieldType, "NotifyPlayerGender Gender type");
            AssertEqual(typeof(long), notifyChangeGenderTime.FieldType, "NotifyPlayerGender ChangeGenderTime type");
            NotifyPlayerGender notify = new()
            {
                Gender = selectedGender,
                ChangeGenderTime = changeGenderTime
            };
            NotifyPlayerGender notifyRoundTrip = MessagePackSerializer.Deserialize<NotifyPlayerGender>(
                MessagePackSerializer.Serialize(notify));
            AssertEqual((long)selectedGender, notifyRoundTrip.Gender, "NotifyPlayerGender Gender MessagePack round-trip");
            AssertEqual(changeGenderTime, notifyRoundTrip.ChangeGenderTime, "NotifyPlayerGender ChangeGenderTime MessagePack round-trip");

            MethodInfo playerDataGetter = RequiredMethod(
                typeof(AscNet.Common.Database.Player),
                $"get_{nameof(AscNet.Common.Database.Player.PlayerData)}",
                BindingFlags.Instance | BindingFlags.Public);
            MethodInfo playerSave = RequiredMethod(
                typeof(AscNet.Common.Database.Player),
                nameof(AscNet.Common.Database.Player.Save),
                BindingFlags.Instance | BindingFlags.Public);
            MethodInfo inventoryDo = RequiredMethod(
                typeof(AscNet.Common.Database.Inventory),
                nameof(AscNet.Common.Database.Inventory.Do),
                BindingFlags.Instance | BindingFlags.Public,
                [typeof(int), typeof(int)]);
            MethodInfo inventorySave = RequiredMethod(
                typeof(AscNet.Common.Database.Inventory),
                nameof(AscNet.Common.Database.Inventory.Save),
                BindingFlags.Instance | BindingFlags.Public);

            MethodInfo changeGenderHandler = GetRegisteredRequestHandlerMethod("ChangePlayerGenderRequest");
            AssertEqual("ChangePlayerGenderRequestHandler", changeGenderHandler.Name, "ChangePlayerGenderRequest registered handler method");
            AssertGenderValidationRejectsOnlyOutsideCurrentClientRange(
                changeGenderHandler,
                requestGender,
                responseCode,
                "ChangePlayerGenderRequestHandler current-client gender validation");
            AssertHandlerSendsResponseCode(
                changeGenderHandler,
                responseCode,
                20002021,
                "ChangePlayerGenderRequestHandler unchanged gender response");
            AssertSameGenderResponseRequiresAlreadySetGender(
                changeGenderHandler,
                requestGender,
                playerDataGenderGetter,
                playerDataChangeGenderTimeGetter,
                responseCode,
                "ChangePlayerGenderRequestHandler unchanged gender guard");
            AssertRequestFieldFeedsSetterBeforePersistence(
                changeGenderHandler,
                requestGender,
                playerDataGenderSetter,
                playerSave,
                "ChangePlayerGenderRequestHandler selected gender persistence");
            AssertLiveGenderRefreshBeforeSuccessResponse(
                changeGenderHandler,
                playerDataGetter,
                playerDataGenderGetter,
                playerDataGenderSetter,
                playerDataChangeGenderTimeGetter,
                playerDataChangeGenderTimeSetter,
                responseGender,
                responseChangeGenderTime,
                responseNextCanChangeTime,
                responsePlayerData,
                notifyGender,
                notifyChangeGenderTime,
                "ChangePlayerGenderRequestHandler live gender refresh");
            AssertFirstGenderSetupRewardPath(
                changeGenderHandler,
                playerDataChangeGenderTimeGetter,
                playerDataChangeGenderTimeSetter,
                responseRewardGoodsList,
                inventoryDo,
                inventorySave,
                playerSave,
                "ChangePlayerGenderRequestHandler first gender setup reward");
        }

        private static void ValidateBoardMutualClientPushCompatibility()
        {
            AssertEqual(true, Session.IsKnownClientPush("BoardMutualRequest"), "Session known client push BoardMutualRequest");
            AssertEqual(false, Session.IsKnownClientPush("DefinitelyUnknownClientPushForCompatibilityTest"), "Session unknown client push");
        }

        private static void ValidateCharacterProgressionPersistenceCompatibility()
        {
            MethodInfo characterSave = RequiredMethod(
                typeof(AscNet.Common.Database.Character),
                "Save",
                BindingFlags.Instance | BindingFlags.Public);
            MethodInfo inventorySave = RequiredMethod(
                typeof(AscNet.Common.Database.Inventory),
                "Save",
                BindingFlags.Instance | BindingFlags.Public);

            PersistenceHandlerContract[] characterProgressionHandlers =
            [
                new("CharacterLevelUpRequest", "CharacterLevelUpRequestHandler"),
                new("CharacterPromoteGradeRequest", "CharacterPromoteGradeRequestHandler"),
                new("CharacterActivateStarRequest", "CharacterActivateStarRequestHandler"),
                new("CharacterPromoteQualityRequest", "CharacterPromoteQualityRequestHandler"),
                new("CharacterUnlockSkillGroupRequest", "CharacterUnlockSkillGroupRequestHandler"),
                new("CharacterUpgradeSkillGroupRequest", "CharacterUpgradeSkillGroupRequestHandler"),
                new("CharacterUnlockEnhanceSkillRequest", "CharacterUnlockEnhanceSkillRequestHandler"),
                new("CharacterUpgradeEnhanceSkillRequest", "CharacterUpgradeEnhanceSkillRequestHandler"),
                new("CharacterExchangeRequest", "CharacterExchangeRequestHandler")
            ];

            foreach (PersistenceHandlerContract handlerContract in characterProgressionHandlers)
            {
                MethodInfo handler = GetRegisteredRequestHandlerMethod(handlerContract.RequestName);
                AssertEqual(handlerContract.HandlerMethodName, handler.Name, $"{handlerContract.RequestName} registered handler method");
                AssertMethodTransitivelyCalls(handler, characterSave, $"{handlerContract.HandlerMethodName} character persistence");
                AssertMethodTransitivelyCalls(handler, inventorySave, $"{handlerContract.HandlerMethodName} inventory persistence");
            }

            MethodInfo fightSettleHandler = GetRegisteredRequestHandlerMethod("FightSettleRequest");
            AssertEqual("FightSettleRequestHandler", fightSettleHandler.Name, "FightSettleRequest registered handler method");
            AssertMethodTransitivelyCalls(fightSettleHandler, characterSave, "FightSettleRequestHandler character card-exp persistence");
        }

        private static void ValidateExpLevelCompatibility()
        {
            const int lotusCharacterId = 1021001;
            const int mainlineStageId = 10010101;

            AscNet.Common.Database.CharacterLevelUpTemplate levelOneTemplate = RequiredCharacterLevelUpTemplate(lotusCharacterId, level: 1);
            AscNet.Common.Database.CharacterLevelUpTemplate levelTwoTemplate = RequiredCharacterLevelUpTemplate(lotusCharacterId, level: 2);
            AscNet.Common.Database.CharacterLevelUpTemplate levelThreeTemplate = RequiredCharacterLevelUpTemplate(lotusCharacterId, level: 3);
            StageTable firstMainlineStage = TableReaderV2.Parse<StageTable>().Single(stage => stage.StageId == mainlineStageId);

            AssertEqual<int?>(null, firstMainlineStage.TeamExp, $"StageTable {mainlineStageId} legacy TeamExp");
            AssertEqual(6, firstMainlineStage.FirstTeamExp ?? throw new InvalidDataException($"StageTable {mainlineStageId}: missing FirstTeamExp."), $"StageTable {mainlineStageId} FirstTeamExp");
            AssertEqual<int?>(null, firstMainlineStage.CardExp, $"StageTable {mainlineStageId} legacy CardExp");
            int firstClearCardExp = firstMainlineStage.FirstCardExp
                ?? throw new InvalidDataException($"StageTable {mainlineStageId}: missing FirstCardExp.");
            AssertEqual(11, firstClearCardExp, $"StageTable {mainlineStageId} FirstCardExp");

            AscNet.Common.Database.Character thresholdNoExpRoster = CreateTestCharacterRoster(lotusCharacterId, level: 1);
            CharacterData thresholdNoExpCharacter = RequiredCharacterData(thresholdNoExpRoster, lotusCharacterId);
            thresholdNoExpCharacter.Exp = (uint)levelOneTemplate.Exp;
            CharacterData thresholdNoExpResult = thresholdNoExpRoster.AddCharacterExp(lotusCharacterId, exp: 0)
                ?? throw new InvalidDataException("AddCharacterExp returned nil for an owned character at the level threshold.");
            AssertEqual(2, thresholdNoExpResult.Level, "AddCharacterExp threshold rollover with zero gained exp level");
            AssertEqual(0U, thresholdNoExpResult.Exp, "AddCharacterExp threshold rollover with zero gained exp carried exp");

            AscNet.Common.Database.Character thresholdBattleExpRoster = CreateTestCharacterRoster(lotusCharacterId, level: 1);
            CharacterData thresholdBattleExpCharacter = RequiredCharacterData(thresholdBattleExpRoster, lotusCharacterId);
            thresholdBattleExpCharacter.Exp = (uint)levelOneTemplate.Exp;
            CharacterData thresholdBattleExpResult = thresholdBattleExpRoster.AddCharacterExp(lotusCharacterId, firstClearCardExp)
                ?? throw new InvalidDataException("AddCharacterExp returned nil for an owned character receiving first-clear battle exp.");
            AssertEqual(2, thresholdBattleExpResult.Level, "AddCharacterExp threshold rollover with battle exp level");
            AssertEqual((uint)firstClearCardExp, thresholdBattleExpResult.Exp, "AddCharacterExp threshold rollover with battle exp carried exp");

            if (levelThreeTemplate.Exp <= 1)
                throw new InvalidDataException($"CharacterLevelUpTemplate: expected level 3 exp threshold to support a positive carry-over assertion, got {levelThreeTemplate.Exp}.");
            int multiLevelCarryExp = Math.Max(1, Math.Min(firstClearCardExp, levelThreeTemplate.Exp - 1));
            AscNet.Common.Database.Character multiLevelRoster = CreateTestCharacterRoster(lotusCharacterId, level: 1);
            CharacterData multiLevelResult = multiLevelRoster.AddCharacterExp(lotusCharacterId, exp: levelOneTemplate.Exp + levelTwoTemplate.Exp + multiLevelCarryExp, maxLvl: 10)
                ?? throw new InvalidDataException("AddCharacterExp returned nil for an owned character receiving enough exp for multiple levels.");
            AssertEqual(3, multiLevelResult.Level, "AddCharacterExp multi-level rollover below commandant cap level");
            AssertEqual((uint)multiLevelCarryExp, multiLevelResult.Exp, "AddCharacterExp multi-level rollover below commandant cap carried exp");

            AscNet.Common.Database.Character cappedRoster = CreateTestCharacterRoster(lotusCharacterId, level: 1);
            CharacterData cappedCharacter = RequiredCharacterData(cappedRoster, lotusCharacterId);
            cappedCharacter.Exp = (uint)levelOneTemplate.Exp;
            CharacterData cappedResult = cappedRoster.AddCharacterExp(lotusCharacterId, exp: levelTwoTemplate.Exp + firstClearCardExp, maxLvl: 2)
                ?? throw new InvalidDataException("AddCharacterExp returned nil for an owned character capped by commandant level.");
            AssertEqual(2, cappedResult.Level, "AddCharacterExp maxLvl cap level");
            AssertEqual((uint)levelTwoTemplate.Exp, cappedResult.Exp, "AddCharacterExp maxLvl cap exp");

            MethodInfo fightSettleHandler = GetRegisteredRequestHandlerMethod("FightSettleRequest");
            AssertEqual("FightSettleRequestHandler", fightSettleHandler.Name, "FightSettleRequest registered handler method");

            MethodInfo expSanityCheck = RequiredMethod(
                typeof(SessionExtensions),
                nameof(SessionExtensions.ExpSanityCheck),
                BindingFlags.Static | BindingFlags.Public,
                [typeof(Session)]);
            MethodInfo playerSave = RequiredMethod(
                typeof(AscNet.Common.Database.Player),
                nameof(AscNet.Common.Database.Player.Save),
                BindingFlags.Instance | BindingFlags.Public);
            MethodInfo inventorySave = RequiredMethod(
                typeof(AscNet.Common.Database.Inventory),
                nameof(AscNet.Common.Database.Inventory.Save),
                BindingFlags.Instance | BindingFlags.Public);
            MethodInfo inventoryDo = RequiredMethod(
                typeof(AscNet.Common.Database.Inventory),
                nameof(AscNet.Common.Database.Inventory.Do),
                BindingFlags.Instance | BindingFlags.Public,
                [typeof(int), typeof(int)]);
            MethodInfo characterSave = RequiredMethod(
                typeof(AscNet.Common.Database.Character),
                nameof(AscNet.Common.Database.Character.Save),
                BindingFlags.Instance | BindingFlags.Public);
            MethodInfo stageSave = RequiredMethod(
                typeof(AscNet.Common.Database.Stage),
                nameof(AscNet.Common.Database.Stage.Save),
                BindingFlags.Instance | BindingFlags.Public);

            MethodInfo levelUpHandler = GetRegisteredRequestHandlerMethod("CharacterLevelUpRequest");
            AssertEqual("CharacterLevelUpRequestHandler", levelUpHandler.Name, "CharacterLevelUpRequest registered handler method");
            AssertLevelUpMaxCapResponsePrecedesInventoryMutation(levelUpHandler, inventoryDo, inventorySave, "CharacterLevelUpRequestHandler commandant cap guard");

            AssertMethodTransitivelyCalls(fightSettleHandler, expSanityCheck, "FightSettleRequestHandler commandant exp sanity settlement");
            AssertMethodTransitivelyCalls(fightSettleHandler, playerSave, "FightSettleRequestHandler player settlement persistence");
            AssertMethodTransitivelyCalls(fightSettleHandler, inventorySave, "FightSettleRequestHandler inventory settlement persistence");
            AssertMethodTransitivelyCalls(fightSettleHandler, characterSave, "FightSettleRequestHandler character settlement persistence");
            AssertMethodTransitivelyCalls(fightSettleHandler, stageSave, "FightSettleRequestHandler stage settlement persistence");
        }

        private static AscNet.Common.Database.Character CreateTestCharacterRoster(int characterId, int level)
        {
            AscNet.Common.Database.Character roster = new()
            {
                Uid = 1,
                Characters = [],
                Equips = [],
                Fashions = []
            };

            roster.AddCharacter((uint)characterId, level);
            return roster;
        }

        private static CharacterData RequiredCharacterData(AscNet.Common.Database.Character roster, int characterId)
        {
            return roster.Characters.SingleOrDefault(character => character.Id == characterId)
                ?? throw new InvalidDataException($"Character roster is missing character {characterId}.");
        }

        private static AscNet.Common.Database.CharacterLevelUpTemplate RequiredCharacterLevelUpTemplate(int characterId, int level)
        {
            CharacterTable characterTable = TableReaderV2.Parse<CharacterTable>().Single(character => character.Id == characterId);
            return AscNet.Common.Database.Character.characterLevelUpTemplates
                .SingleOrDefault(template => template.Level == level && template.Type == characterTable.Type)
                ?? throw new InvalidDataException($"CharacterLevelUpTemplate: missing level {level}, type {characterTable.Type} for character {characterId}.");
        }

        private static void ValidatePr2QualityCompatibility()
        {
            PlayerData playerData = new()
            {
                Id = 7,
                Name = "CompatibilityCommandant",
                NewPlayerTaskActiveDay = 4
            };
            PlayerData playerDataRoundTrip = MessagePackSerializer.Deserialize<PlayerData>(
                MessagePackSerializer.Serialize(playerData));
            AssertEqual(4, playerDataRoundTrip.NewPlayerTaskActiveDay, "PlayerData NewPlayerTaskActiveDay MessagePack round-trip");

            NotifyNewPlayerTaskStatus taskStatus = new()
            {
                NewPlayerTaskActiveDay = 4
            };
            NotifyNewPlayerTaskStatus taskStatusRoundTrip = MessagePackSerializer.Deserialize<NotifyNewPlayerTaskStatus>(
                MessagePackSerializer.Serialize(taskStatus));
            AssertEqual(4, taskStatusRoundTrip.NewPlayerTaskActiveDay, "NotifyNewPlayerTaskStatus NewPlayerTaskActiveDay MessagePack round-trip");

            AscNet.Common.Database.Player player = new();
            AssertEqual(1, player.GatherRewards.Count, "Player initial gather reward count");
            AssertEqual(5, player.GatherRewards[0], "Player initial gather reward id");
            AssertEqual(false, player.AddGatherReward(5), "Player AddGatherReward rejects the already-claimed base reward");
            AssertEqual(1, player.GatherRewards.Count, "Player duplicate base gather reward count");
            AssertEqual(true, player.AddGatherReward(6), "Player AddGatherReward accepts a new reward id");
            AssertEqual(false, player.AddGatherReward(6), "Player AddGatherReward rejects a duplicate new reward id");
            AssertEqual(2, player.GatherRewards.Count, "Player idempotent gather reward count");

            MethodInfo sendPush = RequiredGenericMethodDefinition(
                typeof(Session),
                nameof(Session.SendPush),
                BindingFlags.Instance | BindingFlags.Public,
                parameterCount: 1);
            MethodInfo addGatherReward = RequiredMethod(
                typeof(AscNet.Common.Database.Player),
                nameof(AscNet.Common.Database.Player.AddGatherReward),
                BindingFlags.Instance | BindingFlags.Public,
                [typeof(int)]);
            MethodInfo playerSave = RequiredMethod(
                typeof(AscNet.Common.Database.Player),
                nameof(AscNet.Common.Database.Player.Save),
                BindingFlags.Instance | BindingFlags.Public);
            MethodInfo inventorySave = RequiredMethod(
                typeof(AscNet.Common.Database.Inventory),
                nameof(AscNet.Common.Database.Inventory.Save),
                BindingFlags.Instance | BindingFlags.Public);
            MethodInfo characterSave = RequiredMethod(
                typeof(AscNet.Common.Database.Character),
                nameof(AscNet.Common.Database.Character.Save),
                BindingFlags.Instance | BindingFlags.Public);
            MethodInfo changeNameTimeSetter = typeof(PlayerData).GetProperty(nameof(PlayerData.ChangeNameTime))?.SetMethod
                ?? throw new MissingMethodException(typeof(PlayerData).FullName, $"set_{nameof(PlayerData.ChangeNameTime)}");
            MethodInfo newPlayerTaskActiveDaySetter = typeof(PlayerData).GetProperty(nameof(PlayerData.NewPlayerTaskActiveDay))?.SetMethod
                ?? throw new MissingMethodException(typeof(PlayerData).FullName, $"set_{nameof(PlayerData.NewPlayerTaskActiveDay)}");
            Type rewardHandlerType = RequiredAscNetGameServerType("AscNet.GameServer.Handlers.RewardHandler");
            MethodInfo getRewardGoods = RequiredMethod(
                rewardHandlerType,
                "GetRewardGoods",
                BindingFlags.Static | BindingFlags.Public,
                [typeof(int)]);

            MethodInfo loginHandler = GetRegisteredRequestHandlerMethod("LoginRequest");
            AssertEqual("LoginRequestHandler", loginHandler.Name, "LoginRequest registered handler method");
            AssertMethodTransitivelyCalls(loginHandler, addGatherReward, "LoginRequestHandler base gather reward claim");
            AssertMethodTransitivelyCalls(loginHandler, newPlayerTaskActiveDaySetter, "LoginRequestHandler new-player active-day update");
            AssertMethodTransitivelyCallsGenericMethod(loginHandler, sendPush, typeof(NotifyGatherRewardList), "LoginRequestHandler gather reward list push");
            AssertMethodTransitivelyCallsGenericMethod(loginHandler, sendPush, typeof(NotifyBirthdayPlot), "LoginRequestHandler birthday plot push");
            AssertMethodTransitivelyCallsGenericMethod(loginHandler, sendPush, typeof(NotifyNewPlayerTaskStatus), "LoginRequestHandler new-player task status push");

            MethodInfo changeNameHandler = GetRegisteredRequestHandlerMethod("ChangePlayerNameRequest");
            AssertEqual("ChangePlayerNameRequestHandler", changeNameHandler.Name, "ChangePlayerNameRequest registered handler method");
            AssertMethodTransitivelyCalls(changeNameHandler, changeNameTimeSetter, "ChangePlayerNameRequestHandler ChangeNameTime update");

            MethodInfo changeBirthdayHandler = GetRegisteredRequestHandlerMethod("ChangePlayerBirthdayRequest");
            AssertEqual("ChangePlayerBirthdayRequestHandler", changeBirthdayHandler.Name, "ChangePlayerBirthdayRequest registered handler method");
            AssertMethodTransitivelyCallsGenericMethod(changeBirthdayHandler, sendPush, typeof(NotifyBirthdayPlot), "ChangePlayerBirthdayRequestHandler birthday plot push");

            MethodInfo gatherRewardHandler = GetRegisteredRequestHandlerMethod("GatherRewardRequest");
            AssertEqual("HandleGatherRewardRequestHandler", gatherRewardHandler.Name, "GatherRewardRequest registered handler method");
            AssertMethodTransitivelyCalls(gatherRewardHandler, addGatherReward, "GatherRewardRequest duplicate-safe claim marker");
            AssertCallResultFeedsConditionalBranch(gatherRewardHandler, addGatherReward, "GatherRewardRequest duplicate-safe claim guard");
            AssertMethodTransitivelyCalls(gatherRewardHandler, getRewardGoods, "GatherRewardRequest reward goods resolution");
            AssertMethodTransitivelyCalls(gatherRewardHandler, playerSave, "GatherRewardRequest player persistence");
            AssertMethodTransitivelyCalls(gatherRewardHandler, inventorySave, "GatherRewardRequest inventory persistence");
            AssertMethodTransitivelyCalls(gatherRewardHandler, characterSave, "GatherRewardRequest character persistence");
        }

        private static void ValidateStoryCourseRewardCompatibility()
        {
            const int storyStageId = 10010103;
            const int staleStageFirstRewardId = 10010103;
            const int expectedCourseRewardId = 300000;
            const int expectedCourseShowId = 1031001;
            const int expectedRewardGoodsTemplateId = 1031001;
            const int expectedStoryTaskIdForStage = 102;
            const int currentStoryTaskProgressStageId = 10010104;
            const int expectedCurrentStoryTaskId = 103;

            List<CourseTable> courseRows = TableReaderV2.Parse<CourseTable>()
                .Where(course => course.StageId == storyStageId)
                .ToList();
            AssertEqual(1, courseRows.Count, $"CourseTable rows for StageId {storyStageId}");

            CourseTable courseTable = courseRows[0];
            AssertEqual(expectedCourseRewardId, courseTable.RewardId, $"CourseTable {storyStageId} RewardId");
            AssertEqual(expectedCourseShowId, courseTable.ShowId, $"CourseTable {storyStageId} ShowId");

            StageTable stageTable = TableReaderV2.Parse<StageTable>().FirstOrDefault(stage => stage.StageId == storyStageId)
                ?? throw new InvalidDataException($"StageTable: missing current story stage {storyStageId}.");
            int stageFirstRewardId = stageTable.FirstRewardId
                ?? throw new InvalidDataException($"StageTable {storyStageId}: missing FirstRewardId.");
            AssertEqual(staleStageFirstRewardId, stageFirstRewardId, $"StageTable {storyStageId} stale FirstRewardId");
            if (stageFirstRewardId == courseTable.RewardId)
                throw new InvalidDataException($"CourseTable {storyStageId}: RewardId must not be taken from StageTable.FirstRewardId.");

            List<RewardGoodsTable> rewardGoodsTables = TableReaderV2.Parse<RewardGoodsTable>();
            List<RewardGoodsTable> courseRewardGoods = ResolveRewardGoods(courseTable.RewardId, rewardGoodsTables, $"CourseTable {storyStageId} RewardId");
            if (!courseRewardGoods.Any(goods => goods.TemplateId == expectedRewardGoodsTemplateId))
                throw new InvalidDataException($"CourseTable {storyStageId}: expected RewardId {courseTable.RewardId} to resolve to RewardGoods template {expectedRewardGoodsTemplateId}.");

            List<RewardGoodsTable> staleFirstRewardGoods = ResolveRewardGoods(stageFirstRewardId, rewardGoodsTables, $"StageTable {storyStageId} FirstRewardId");
            if (staleFirstRewardGoods.Any(goods => goods.TemplateId == expectedRewardGoodsTemplateId))
                throw new InvalidDataException($"StageTable {storyStageId}: stale FirstRewardId path unexpectedly resolves to course RewardGoods template {expectedRewardGoodsTemplateId}.");

            ValidateStoryTaskProgressCompatibility(storyStageId, expectedStoryTaskIdForStage);
            ValidateStoryTaskProgressCompatibility(currentStoryTaskProgressStageId, expectedCurrentStoryTaskId);

            MethodInfo stageSave = RequiredMethod(
                typeof(AscNet.Common.Database.Stage),
                "Save",
                BindingFlags.Instance | BindingFlags.Public);
            MethodInfo inventorySave = RequiredMethod(
                typeof(AscNet.Common.Database.Inventory),
                "Save",
                BindingFlags.Instance | BindingFlags.Public);
            MethodInfo characterSave = RequiredMethod(
                typeof(AscNet.Common.Database.Character),
                "Save",
                BindingFlags.Instance | BindingFlags.Public);
            MethodInfo stageAddCourse = RequiredMethod(
                typeof(AscNet.Common.Database.Stage),
                "AddCourse",
                BindingFlags.Instance | BindingFlags.Public,
                [typeof(uint)]);
            MethodInfo stageAddFinishedTask = RequiredMethod(
                typeof(AscNet.Common.Database.Stage),
                "AddFinishedTask",
                BindingFlags.Instance | BindingFlags.Public,
                [typeof(int)]);
            MethodInfo tableParse = RequiredMethod(
                typeof(TableReaderV2),
                nameof(TableReaderV2.Parse),
                BindingFlags.Static | BindingFlags.Public);
            Type taskModule = RequiredAscNetGameServerType("AscNet.GameServer.Handlers.TaskModule");
            MethodInfo sendStoryTaskSync = RequiredMethod(
                taskModule,
                "SendStoryTaskSync",
                BindingFlags.Static | BindingFlags.Public,
                [typeof(Session)]);

            MethodInfo courseRewardHandler = GetRegisteredRequestHandlerMethod("GetCourseRewardRequest");
            AssertEqual("GetCourseRewardRequestHandler", courseRewardHandler.Name, "GetCourseRewardRequest registered handler method");
            AssertMethodTransitivelyCallsGenericMethod(courseRewardHandler, tableParse, typeof(CourseTable), "GetCourseRewardRequestHandler course table lookup");
            AssertMethodTransitivelyCallsGenericMethod(courseRewardHandler, tableParse, typeof(RewardTable), "GetCourseRewardRequestHandler reward table lookup");
            AssertMethodTransitivelyCallsGenericMethod(courseRewardHandler, tableParse, typeof(RewardGoodsTable), "GetCourseRewardRequestHandler reward goods lookup");
            AssertMethodDoesNotTransitivelyCallGenericMethod(courseRewardHandler, tableParse, typeof(StageTable), "GetCourseRewardRequestHandler stale stage first-clear lookup");
            AssertMethodTransitivelyCalls(courseRewardHandler, stageAddCourse, "GetCourseRewardRequestHandler course claim marker");
            AssertMethodTransitivelyCalls(courseRewardHandler, stageSave, "GetCourseRewardRequestHandler stage course persistence");
            AssertMethodTransitivelyCalls(courseRewardHandler, inventorySave, "GetCourseRewardRequestHandler inventory reward persistence");
            AssertMethodTransitivelyCalls(courseRewardHandler, characterSave, "GetCourseRewardRequestHandler character reward persistence");

            MethodInfo finishTaskHandler = GetRegisteredRequestHandlerMethod("FinishTaskRequest");
            AssertEqual("FinishTaskRequestHandler", finishTaskHandler.Name, "FinishTaskRequest registered handler method");
            AssertMethodTransitivelyCallsGenericMethod(finishTaskHandler, tableParse, typeof(StoryTaskTable), "FinishTaskRequestHandler story task lookup");
            AssertMethodTransitivelyCallsGenericMethod(finishTaskHandler, tableParse, typeof(StoryTaskConditionTable), "FinishTaskRequestHandler story task condition lookup");
            AssertMethodTransitivelyCallsGenericMethod(finishTaskHandler, tableParse, typeof(RewardTable), "FinishTaskRequestHandler reward table lookup");
            AssertMethodTransitivelyCallsGenericMethod(finishTaskHandler, tableParse, typeof(RewardGoodsTable), "FinishTaskRequestHandler reward goods lookup");
            AssertMethodTransitivelyCalls(finishTaskHandler, stageAddFinishedTask, "FinishTaskRequestHandler finished task marker");
            AssertMethodTransitivelyCalls(finishTaskHandler, stageSave, "FinishTaskRequestHandler stage task persistence");
            AssertMethodTransitivelyCalls(finishTaskHandler, inventorySave, "FinishTaskRequestHandler inventory reward persistence");
            AssertMethodTransitivelyCalls(finishTaskHandler, characterSave, "FinishTaskRequestHandler character reward persistence");
            AssertMethodTransitivelyCalls(finishTaskHandler, sendStoryTaskSync, "FinishTaskRequestHandler story task sync push");

            MethodInfo finishMultiTaskHandler = GetRegisteredRequestHandlerMethod("FinishMultiTaskRequest");
            AssertEqual("FinishMultiTaskRequestHandler", finishMultiTaskHandler.Name, "FinishMultiTaskRequest registered handler method");
            AssertMethodTransitivelyCallsGenericMethod(finishMultiTaskHandler, tableParse, typeof(StoryTaskTable), "FinishMultiTaskRequestHandler story task lookup");
            AssertMethodTransitivelyCallsGenericMethod(finishMultiTaskHandler, tableParse, typeof(StoryTaskConditionTable), "FinishMultiTaskRequestHandler story task condition lookup");
            AssertMethodTransitivelyCallsGenericMethod(finishMultiTaskHandler, tableParse, typeof(RewardTable), "FinishMultiTaskRequestHandler reward table lookup");
            AssertMethodTransitivelyCallsGenericMethod(finishMultiTaskHandler, tableParse, typeof(RewardGoodsTable), "FinishMultiTaskRequestHandler reward goods lookup");
            AssertMethodTransitivelyCalls(finishMultiTaskHandler, stageAddFinishedTask, "FinishMultiTaskRequestHandler finished task marker");
            AssertMethodTransitivelyCalls(finishMultiTaskHandler, stageSave, "FinishMultiTaskRequestHandler stage task persistence");
            AssertMethodTransitivelyCalls(finishMultiTaskHandler, inventorySave, "FinishMultiTaskRequestHandler inventory reward persistence");
            AssertMethodTransitivelyCalls(finishMultiTaskHandler, characterSave, "FinishMultiTaskRequestHandler character reward persistence");
            AssertMethodTransitivelyCalls(finishMultiTaskHandler, sendStoryTaskSync, "FinishMultiTaskRequestHandler story task sync push");

            MethodInfo fightSettleHandler = GetRegisteredRequestHandlerMethod("FightSettleRequest");
            AssertEqual("FightSettleRequestHandler", fightSettleHandler.Name, "FightSettleRequest registered handler method");
            AssertMethodDoesNotTransitivelyCall(fightSettleHandler, stageAddCourse, "FightSettleRequestHandler course claim marker");
        }

        private static List<RewardGoodsTable> ResolveRewardGoods(int rewardId, IReadOnlyCollection<RewardGoodsTable> rewardGoodsTables, string name)
        {
            RewardTable rewardTable = TableReaderV2.Parse<RewardTable>().FirstOrDefault(reward => reward.Id == rewardId)
                ?? throw new InvalidDataException($"{name}: missing RewardTable id {rewardId}.");
            if (rewardTable.SubIds.Count == 0)
                throw new InvalidDataException($"{name}: RewardTable {rewardId} has no SubIds.");

            HashSet<int> rewardSubIds = rewardTable.SubIds.ToHashSet();
            List<RewardGoodsTable> rewardGoods = rewardGoodsTables
                .Where(goods => rewardSubIds.Contains(goods.Id))
                .ToList();
            int[] missingRewardGoodsIds = rewardSubIds.Except(rewardGoods.Select(goods => goods.Id)).ToArray();
            if (missingRewardGoodsIds.Length > 0)
                throw new InvalidDataException($"{name}: missing RewardGoods rows for SubIds {string.Join(", ", missingRewardGoodsIds)}.");

            return rewardGoods;
        }

        private static void ValidateStoryTaskProgressCompatibility(int passedStageId, int expectedStoryTaskId)
        {
            const int taskStateAchieved = 3;
            const int taskStateFinish = 4;

            StoryTaskConditionTable storyTaskCondition = TableReaderV2.Parse<StoryTaskConditionTable>()
                .Single(condition => condition.Params.Contains(passedStageId));
            AssertEqual(expectedStoryTaskId, storyTaskCondition.Id, $"StoryTaskCondition stage {passedStageId} Id");

            StoryTaskTable storyTask = TableReaderV2.Parse<StoryTaskTable>()
                .Single(task => task.Condition == storyTaskCondition.Id);
            AssertEqual(expectedStoryTaskId, storyTask.Id, $"StoryTask stage {passedStageId} task id");
            AssertEqual(1, storyTask.Result, $"StoryTask {expectedStoryTaskId} required progress");

            Session session = CreateStoryTaskProgressSession(passedStageId);
            LoginTask achievedTask = RequiredStoryLoginTask(BuildStoryTaskData(session), expectedStoryTaskId);
            AssertEqual(taskStateAchieved, achievedTask.State, $"BuildStoryTaskData task {expectedStoryTaskId} achieved state");
            AssertEqual(1, achievedTask.Schedule.Count, $"BuildStoryTaskData task {expectedStoryTaskId} schedule count");
            AssertEqual((uint)storyTaskCondition.Id, achievedTask.Schedule[0].Id, $"BuildStoryTaskData task {expectedStoryTaskId} condition id");
            AssertEqual(storyTask.Result, achievedTask.Schedule[0].Value, $"BuildStoryTaskData task {expectedStoryTaskId} progress value");

            if (!session.stage.AddFinishedTask(expectedStoryTaskId))
                throw new InvalidDataException($"Stage failed to mark story task {expectedStoryTaskId} finished.");

            LoginTask finishedTask = RequiredStoryLoginTask(BuildStoryTaskData(session), expectedStoryTaskId);
            AssertEqual(taskStateFinish, finishedTask.State, $"BuildStoryTaskData task {expectedStoryTaskId} finished state");
            AssertEqual(storyTask.Result, finishedTask.Schedule[0].Value, $"BuildStoryTaskData task {expectedStoryTaskId} finished progress value");
        }

        private static Session CreateStoryTaskProgressSession(int passedStageId)
        {
            Session session = (Session)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(Session));
            session.stage = new AscNet.Common.Database.Stage
            {
                Uid = -1,
                Stages = new(),
                Course = new(),
                FinishedTasks = new()
            };
            session.stage.AddStage(new StageDatum
            {
                StageId = passedStageId,
                StarsMark = 7,
                Passed = true,
                PassTimesToday = 0,
                PassTimesTotal = 1
            });
            return session;
        }

        private static List<LoginTask> BuildStoryTaskData(Session session)
        {
            Type taskModule = RequiredAscNetGameServerType("AscNet.GameServer.Handlers.TaskModule");
            MethodInfo buildStoryTaskData = RequiredMethod(
                taskModule,
                "BuildStoryTaskData",
                BindingFlags.Static | BindingFlags.Public,
                [typeof(Session)]);

            return (List<LoginTask>?)buildStoryTaskData.Invoke(null, [session])
                ?? throw new InvalidDataException("TaskModule.BuildStoryTaskData returned nil.");
        }

        private static LoginTask RequiredStoryLoginTask(IEnumerable<LoginTask> tasks, int taskId)
        {
            return tasks.SingleOrDefault(task => task.Id == (uint)taskId)
                ?? throw new InvalidDataException($"BuildStoryTaskData did not include story task {taskId}.");
        }

        private sealed record PersistenceHandlerContract(string RequestName, string HandlerMethodName);

        private static void ValidateBossSingleLoginCompatibilityShape()
        {
            NotifyFubenBossSingleData notification = new()
            {
                FubenBossSingleData = new()
                {
                    ActivityNo = 1,
                    TotalScore = 0,
                    MaxScore = 0,
                    OldLevelType = 1,
                    LevelType = 1,
                    ChallengeCount = 0,
                    RemainTime = 3600 * 24,
                    AutoFightCount = 0,
                    RankPlatform = 0,
                    AfreshId = 0,
                    ChallengeLevelType = 0,
                    IsResetOpen = false
                }
            };

            NotifyFubenBossSingleData roundTrip = MessagePackSerializer.Deserialize<NotifyFubenBossSingleData>(
                MessagePackSerializer.Serialize(notification));

            NotifyFubenBossSingleData.NotifyFubenBossSingleDataFubenBossSingleData bossSingleData = roundTrip.FubenBossSingleData
                ?? throw new InvalidDataException("NotifyFubenBossSingleData FubenBossSingleData serialized as nil.");
            AssertEmptyList(bossSingleData.CharacterPoints, "NotifyFubenBossSingleData FubenBossSingleData.CharacterPoints");
            AssertEmptyList(bossSingleData.HistoryList, "NotifyFubenBossSingleData FubenBossSingleData.HistoryList");
            AssertEmptyList(bossSingleData.RewardIds, "NotifyFubenBossSingleData FubenBossSingleData.RewardIds");
            AssertEmptyList(bossSingleData.BossList, "NotifyFubenBossSingleData FubenBossSingleData.BossList");
            AssertEmptyList(bossSingleData.TrialStageInfoList, "NotifyFubenBossSingleData FubenBossSingleData.TrialStageInfoList");
            AssertEmptyList(bossSingleData.BestiraryStageInfoList, "NotifyFubenBossSingleData FubenBossSingleData.BestiraryStageInfoList");
            AssertEmptyList(bossSingleData.ChallengeStageHistoryList, "NotifyFubenBossSingleData FubenBossSingleData.ChallengeStageHistoryList");
            AssertEmptyList(bossSingleData.StageRecordList, "NotifyFubenBossSingleData FubenBossSingleData.StageRecordList");
            AssertEqual(false, bossSingleData.IsResetOpen, "NotifyFubenBossSingleData FubenBossSingleData.IsResetOpen");
            AssertEqual(0, bossSingleData.AfreshId, "NotifyFubenBossSingleData FubenBossSingleData.AfreshId");
            AssertEqual(1, bossSingleData.LevelType, "NotifyFubenBossSingleData FubenBossSingleData.LevelType");
            AssertEqual(1, bossSingleData.OldLevelType, "NotifyFubenBossSingleData FubenBossSingleData.OldLevelType");
            if (bossSingleData.RemainTime == 0)
                throw new InvalidDataException("NotifyFubenBossSingleData FubenBossSingleData.RemainTime: expected a positive value.");
        }

        private static void ValidateCurrentClientGuideTableCompatibility()
        {
            List<GuideGroupTable> guideGroups = TableReaderV2.Parse<GuideGroupTable>();
            if (guideGroups.Count <= 500)
                throw new InvalidDataException($"GuideGroupTable: expected materially more than the stale 241-row table, got {guideGroups.Count} rows.");

            int[] requiredGuideGroupIds = [100004, 64764, 64772];
            foreach (int guideGroupId in requiredGuideGroupIds)
            {
                if (!guideGroups.Any(guideGroup => guideGroup.Id == guideGroupId))
                    throw new InvalidDataException($"GuideGroupTable: missing current-client guide group id {guideGroupId}.");
            }

            List<GuideFightTable> guideFights = TableReaderV2.Parse<GuideFightTable>();
            if (!guideFights.Any(guideFight => guideFight.StageId == 10010005))
                throw new InvalidDataException("GuideFightTable: missing current tutorial stage 10010005.");
        }

        private static void ValidateRequestHandlerRegistration(string requestName)
        {
            Dictionary<string, RequestPacketHandlerDelegate> handlersSnapshot = new(PacketFactory.ReqHandlers);

            try
            {
                PacketFactory.ReqHandlers.Remove(requestName);
                PacketFactory.LoadPacketHandlers();

                RequestPacketHandlerDelegate? handler = PacketFactory.GetRequestPacketHandler(requestName);
                if (handler is null)
                    throw new InvalidDataException($"PacketFactory did not register {requestName}.");
            }
            finally
            {
                PacketFactory.ReqHandlers.Clear();
                foreach (KeyValuePair<string, RequestPacketHandlerDelegate> handler in handlersSnapshot)
                    PacketFactory.ReqHandlers.Add(handler.Key, handler.Value);
            }
        }

        private static MethodInfo GetRegisteredRequestHandlerMethod(string requestName)
        {
            Dictionary<string, RequestPacketHandlerDelegate> handlersSnapshot = new(PacketFactory.ReqHandlers);

            try
            {
                PacketFactory.ReqHandlers.Remove(requestName);
                PacketFactory.LoadPacketHandlers();

                RequestPacketHandlerDelegate? handler = PacketFactory.GetRequestPacketHandler(requestName);
                if (handler is null)
                    throw new InvalidDataException($"PacketFactory did not register {requestName}.");

                return handler.Method;
            }
            finally
            {
                PacketFactory.ReqHandlers.Clear();
                foreach (KeyValuePair<string, RequestPacketHandlerDelegate> handler in handlersSnapshot)
                    PacketFactory.ReqHandlers.Add(handler.Key, handler.Value);
            }
        }

        private static MethodInfo RequiredMethod(Type declaringType, string name, BindingFlags bindingFlags)
        {
            return RequiredMethod(declaringType, name, bindingFlags, Type.EmptyTypes);
        }

        private static MethodInfo RequiredMethod(Type declaringType, string name, BindingFlags bindingFlags, Type[] parameterTypes)
        {
            return declaringType.GetMethod(name, bindingFlags, binder: null, types: parameterTypes, modifiers: null)
                ?? throw new MissingMethodException(declaringType.FullName, name);
        }

        private static MethodInfo RequiredGenericMethodDefinition(Type declaringType, string name, BindingFlags bindingFlags, int parameterCount)
        {
            MethodInfo[] matches = declaringType.GetMethods(bindingFlags)
                .Where(method => method.Name == name && method.IsGenericMethodDefinition && method.GetParameters().Length == parameterCount)
                .ToArray();

            return matches.Length switch
            {
                1 => matches[0],
                0 => throw new MissingMethodException(declaringType.FullName, name),
                _ => throw new AmbiguousMatchException($"{declaringType.FullName}.{name} has {matches.Length} generic overloads with {parameterCount} parameters.")
            };
        }

        private static Type RequiredAscNetGameServerType(string fullName)
        {
            return typeof(PacketFactory).Assembly.GetType(fullName, throwOnError: true)
                ?? throw new TypeLoadException(fullName);
        }

        private static void AssertMethodTransitivelyCalls(MethodInfo method, MethodInfo target, string name)
        {
            if (!CallsTargetTransitively(method, target, []))
                throw new InvalidDataException($"{name}: expected {method.DeclaringType?.FullName}.{method.Name} to call {target.DeclaringType?.FullName}.{target.Name} directly or through a private helper.");
        }

        private static void AssertMethodDoesNotTransitivelyCall(MethodInfo method, MethodInfo target, string name)
        {
            if (CallsTargetTransitively(method, target, []))
                throw new InvalidDataException($"{name}: expected {method.DeclaringType?.FullName}.{method.Name} not to call {target.DeclaringType?.FullName}.{target.Name} directly or through a private helper.");
        }

        private static void AssertMethodTransitivelyCallsGenericMethod(MethodInfo method, MethodInfo genericMethodDefinition, Type genericArgument, string name)
        {
            if (!CallsGenericTargetTransitively(method, genericMethodDefinition, genericArgument, []))
                throw new InvalidDataException($"{name}: expected {method.DeclaringType?.FullName}.{method.Name} to call {FormatGenericMethod(genericMethodDefinition, genericArgument)} directly or through a private helper.");
        }

        private static void AssertMethodDoesNotTransitivelyCallGenericMethod(MethodInfo method, MethodInfo genericMethodDefinition, Type genericArgument, string name)
        {
            if (CallsGenericTargetTransitively(method, genericMethodDefinition, genericArgument, []))
                throw new InvalidDataException($"{name}: expected {method.DeclaringType?.FullName}.{method.Name} not to call {FormatGenericMethod(genericMethodDefinition, genericArgument)} directly or through a private helper.");
        }

        private static bool CallsGenericTargetTransitively(MethodInfo method, MethodInfo genericMethodDefinition, Type genericArgument, HashSet<string> visited)
        {
            if (!visited.Add(MethodKey(method)))
                return false;

            foreach (MethodBase calledMethod in CalledMethods(method))
            {
                if (GenericMethodsMatch(calledMethod, genericMethodDefinition, genericArgument))
                    return true;

                if (calledMethod is MethodInfo nestedMethod && CanTraversePrivateHelper(method, nestedMethod) && CallsGenericTargetTransitively(nestedMethod, genericMethodDefinition, genericArgument, visited))
                    return true;
            }

            return false;
        }

        private static bool CallsTargetTransitively(MethodInfo method, MethodInfo target, HashSet<string> visited)
        {
            if (!visited.Add(MethodKey(method)))
                return false;

            foreach (MethodBase calledMethod in CalledMethods(method))
            {
                if (MethodsMatch(calledMethod, target))
                    return true;

                if (calledMethod is MethodInfo nestedMethod && CanTraversePrivateHelper(method, nestedMethod) && CallsTargetTransitively(nestedMethod, target, visited))
                    return true;
            }

            return false;
        }

        private static bool CanTraversePrivateHelper(MethodInfo method, MethodInfo candidate)
        {
            Type? methodType = method.DeclaringType;
            Type? candidateType = candidate.DeclaringType;
            if (methodType is null || candidateType is null)
                return false;

            bool sameTypeHelper = candidateType == methodType;
            bool generatedClosureHelper = candidateType.DeclaringType == methodType
                || methodType.DeclaringType == candidateType
                || (methodType.DeclaringType is not null && methodType.DeclaringType == candidateType.DeclaringType);
            return sameTypeHelper
                ? candidate.IsPrivate
                : generatedClosureHelper && !candidate.IsPublic;
        }

        private static void AssertCallResultFeedsConditionalBranch(MethodInfo method, MethodInfo target, string name)
        {
            byte[] il = method.GetMethodBody()?.GetILAsByteArray()
                ?? throw new InvalidDataException($"{method.DeclaringType?.FullName}.{method.Name}: method body is unavailable.");
            Module module = method.Module;
            Type[] typeArguments = method.DeclaringType?.GetGenericArguments() ?? Type.EmptyTypes;
            Type[] methodArguments = method.GetGenericArguments();

            for (int offset = 0; offset < il.Length;)
            {
                OpCode opCode = ReadOpCode(il, ref offset);
                if (opCode.OperandType == OperandType.InlineMethod)
                {
                    int metadataToken = BitConverter.ToInt32(il, offset);
                    offset += 4;

                    MethodBase? calledMethod;
                    try
                    {
                        calledMethod = module.ResolveMethod(metadataToken, typeArguments, methodArguments);
                    }
                    catch (ArgumentException)
                    {
                        continue;
                    }

                    if (calledMethod is not null && MethodsMatch(calledMethod, target))
                    {
                        if (NextMeaningfulOpCodeIsConditionalBranch(il, offset))
                            return;

                        throw new InvalidDataException($"{name}: expected {method.DeclaringType?.FullName}.{method.Name} to branch on {target.DeclaringType?.FullName}.{target.Name}'s return value.");
                    }
                }
                else
                {
                    offset += OperandSize(opCode.OperandType, il, offset);
                }
            }

            throw new InvalidDataException($"{name}: expected {method.DeclaringType?.FullName}.{method.Name} to call {target.DeclaringType?.FullName}.{target.Name}.");
        }

        private static void AssertLevelUpMaxCapResponsePrecedesInventoryMutation(MethodInfo method, MethodInfo inventoryDo, MethodInfo inventorySave, string name)
        {
            FieldInfo responseCode = typeof(CharacterLevelUpResponse).GetField(nameof(CharacterLevelUpResponse.Code), BindingFlags.Instance | BindingFlags.Public)
                ?? throw new MissingFieldException(typeof(CharacterLevelUpResponse).FullName, nameof(CharacterLevelUpResponse.Code));
            MethodInfo sendResponse = RequiredGenericMethodDefinition(
                typeof(Session),
                nameof(Session.SendResponse),
                BindingFlags.Instance | BindingFlags.Public,
                parameterCount: 2);

            List<IlInstruction> instructions = ReadIlInstructions(method).ToList();
            int maxLevelCodeIndex = FindFieldAssignmentIndex(instructions, responseCode, expectedValue: 20009014);
            if (maxLevelCodeIndex < 0)
                throw new InvalidDataException($"{name}: expected {method.DeclaringType?.FullName}.{method.Name} to assign CharacterLevelUpResponse.Code = 20009014.");

            int maxLevelResponseIndex = FindGenericCallIndex(instructions, sendResponse, typeof(CharacterLevelUpResponse), startIndex: maxLevelCodeIndex + 1);
            if (maxLevelResponseIndex < 0)
                throw new InvalidDataException($"{name}: expected CharacterLevelUpResponse.Code = 20009014 to feed Session.SendResponse<CharacterLevelUpResponse>.");

            if (!HasConditionalBranchGuard(instructions, maxLevelCodeIndex, maxLevelResponseIndex))
                throw new InvalidDataException($"{name}: expected CharacterLevelUpResponse.Code = 20009014 to be reached through a conditional commandant-cap branch.");

            int firstInventoryMutationIndex = FindFirstInventoryMutationIndex(instructions, inventoryDo, inventorySave);
            if (firstInventoryMutationIndex < 0)
                throw new InvalidDataException($"{name}: expected the success path to consume inventory or save inventory changes.");

            if (firstInventoryMutationIndex <= maxLevelResponseIndex)
                throw new InvalidDataException($"{name}: expected the max-level response to be sent before inventory consumption/save.");

            if (!PathExits(instructions, maxLevelResponseIndex + 1))
                throw new InvalidDataException($"{name}: expected the max-level response path to exit before reaching inventory consumption/save.");
        }

        private static void AssertRequestFieldFeedsSetterBeforePersistence(MethodInfo method, FieldInfo sourceField, MethodInfo targetSetter, MethodInfo persistenceMethod, string name)
        {
            List<IlInstruction> instructions = ReadIlInstructions(method).ToList();
            int setterIndex = instructions.FindIndex(instruction => instruction.Operand is MethodBase calledMethod && MethodsMatch(calledMethod, targetSetter));
            if (setterIndex < 0)
                throw new InvalidDataException($"{name}: expected {method.DeclaringType?.FullName}.{method.Name} to assign through {targetSetter.DeclaringType?.FullName}.{targetSetter.Name}.");

            bool loadsSourceField = false;
            for (int previousIndex = setterIndex - 1; previousIndex >= 0 && previousIndex >= setterIndex - 8; previousIndex--)
            {
                if (instructions[previousIndex].Operand is FieldInfo loadedField && FieldsMatch(loadedField, sourceField))
                {
                    loadsSourceField = true;
                    break;
                }
            }

            if (!loadsSourceField)
                throw new InvalidDataException($"{name}: expected assignment through {targetSetter.DeclaringType?.FullName}.{targetSetter.Name} to use {sourceField.DeclaringType?.FullName}.{sourceField.Name}.");

            int persistenceIndex = instructions.FindIndex(setterIndex + 1, instruction => instruction.Operand is MethodBase calledMethod && MethodsMatch(calledMethod, persistenceMethod));
            if (persistenceIndex < 0)
                throw new InvalidDataException($"{name}: expected {persistenceMethod.DeclaringType?.FullName}.{persistenceMethod.Name} after assigning the selected value.");
        }

        private static void AssertHandlerSendsResponseCode(MethodInfo method, FieldInfo responseCodeField, int expectedCode, string name)
        {
            MethodInfo sendResponse = RequiredGenericMethodDefinition(
                typeof(Session),
                nameof(Session.SendResponse),
                BindingFlags.Instance | BindingFlags.Public,
                parameterCount: 2);
            List<IlInstruction> instructions = ReadIlInstructions(method).ToList();

            int codeIndex = FindFieldAssignmentIndex(instructions, responseCodeField, expectedCode);
            if (codeIndex < 0)
                throw new InvalidDataException($"{name}: expected {method.DeclaringType?.FullName}.{method.Name} to assign {responseCodeField.DeclaringType?.FullName}.{responseCodeField.Name} = {expectedCode}.");

            Type responseType = responseCodeField.DeclaringType
                ?? throw new InvalidDataException($"{name}: response code field has no declaring type.");
            int responseIndex = FindGenericCallIndex(instructions, sendResponse, responseType, codeIndex + 1);
            if (responseIndex < 0)
                throw new InvalidDataException($"{name}: expected response code {expectedCode} to feed Session.SendResponse<{responseType.FullName}>.");

            if (!HasConditionalBranchGuard(instructions, codeIndex, responseIndex))
                throw new InvalidDataException($"{name}: expected response code {expectedCode} to be reached through a conditional validation branch.");

            if (!PathExits(instructions, responseIndex + 1))
                throw new InvalidDataException($"{name}: expected response code {expectedCode} path to exit before the success path.");
        }

        private static void AssertGenderValidationRejectsOnlyOutsideCurrentClientRange(MethodInfo method, FieldInfo requestGenderField, FieldInfo responseCodeField, string name)
        {
            MethodInfo sendResponse = RequiredGenericMethodDefinition(
                typeof(Session),
                nameof(Session.SendResponse),
                BindingFlags.Instance | BindingFlags.Public,
                parameterCount: 2);
            List<IlInstruction> instructions = ReadIlInstructions(method).ToList();

            int invalidCodeIndex = FindFieldAssignmentIndex(instructions, responseCodeField, expectedValue: 20002020);
            if (invalidCodeIndex < 0)
                throw new InvalidDataException($"{name}: expected invalid gender response code 20002020.");

            Type responseType = responseCodeField.DeclaringType
                ?? throw new InvalidDataException($"{name}: response code field has no declaring type.");
            int invalidResponseIndex = FindGenericCallIndex(instructions, sendResponse, responseType, invalidCodeIndex + 1);
            if (invalidResponseIndex < 0)
                throw new InvalidDataException($"{name}: expected invalid gender response code 20002020 to feed Session.SendResponse<{responseType.FullName}>.");

            bool hasNormalizedRangeGuard = HasNormalizedInclusiveRangeGuard(
                instructions,
                requestGenderField,
                minimum: 1,
                maximum: 3,
                invalidCodeIndex,
                invalidResponseIndex);
            bool hasLowerBoundGuard = HasGenderBoundBranch(instructions, requestGenderField, bound: 1, invalidCodeIndex, invalidResponseIndex, lowerBound: true);
            bool hasUpperBoundGuard = HasGenderBoundBranch(instructions, requestGenderField, bound: 3, invalidCodeIndex, invalidResponseIndex, lowerBound: false);

            if (!hasNormalizedRangeGuard && !hasLowerBoundGuard)
                throw new InvalidDataException($"{name}: expected gender 0 to reach invalid response 20002020 while gender 1 continues.");

            if (!hasNormalizedRangeGuard && !hasUpperBoundGuard)
                throw new InvalidDataException($"{name}: expected gender 4 to reach invalid response 20002020 while current-client gender 3 continues.");

            if (!PathExits(instructions, invalidResponseIndex + 1))
                throw new InvalidDataException($"{name}: expected invalid gender response path to exit before success handling.");
        }

        private static void AssertSameGenderResponseRequiresAlreadySetGender(MethodInfo method, FieldInfo requestGenderField, MethodInfo playerGenderGetter, MethodInfo playerChangeGenderTimeGetter, FieldInfo responseCodeField, string name)
        {
            List<IlInstruction> instructions = ReadIlInstructions(method).ToList();

            int sameGenderCodeIndex = FindFieldAssignmentIndex(instructions, responseCodeField, expectedValue: 20002021);
            if (sameGenderCodeIndex < 0)
                throw new InvalidDataException($"{name}: expected unchanged gender response code 20002021.");

            if (!HasMethodCallComparedToConstantBefore(instructions, playerGenderGetter, expectedValue: 0, endIndex: sameGenderCodeIndex))
                throw new InvalidDataException($"{name}: expected same-gender rejection to be gated by current PlayerData.Gender being already set (> 0).");

            if (!HasMethodCallStrictlyPositiveComparisonBefore(instructions, playerChangeGenderTimeGetter, endIndex: sameGenderCodeIndex))
                throw new InvalidDataException($"{name}: expected same-gender rejection to require PlayerData.ChangeGenderTime > 0 before returning 20002021.");

            if (!HasSameGenderComparisonBefore(instructions, playerGenderGetter, requestGenderField, sameGenderCodeIndex))
                throw new InvalidDataException($"{name}: expected response code 20002021 to be guarded by comparing current PlayerData.Gender with ChangePlayerGenderRequest.Gender.");
        }

        private static void AssertFirstGenderSetupRewardPath(
            MethodInfo method,
            MethodInfo changeGenderTimeGetter,
            MethodInfo changeGenderTimeSetter,
            FieldInfo responseRewardGoodsListField,
            MethodInfo inventoryDo,
            MethodInfo inventorySave,
            MethodInfo playerSave,
            string name)
        {
            MethodInfo sendPush = RequiredGenericMethodDefinition(
                typeof(Session),
                nameof(Session.SendPush),
                BindingFlags.Instance | BindingFlags.Public,
                parameterCount: 1);
            MethodInfo sendResponse = RequiredGenericMethodDefinition(
                typeof(Session),
                nameof(Session.SendResponse),
                BindingFlags.Instance | BindingFlags.Public,
                parameterCount: 2);
            MethodInfo rewardGoodsListAdd = RequiredMethod(
                typeof(List<RewardGoods>),
                nameof(List<RewardGoods>.Add),
                BindingFlags.Instance | BindingFlags.Public,
                [typeof(RewardGoods)]);
            MethodInfo rewardGoodsRewardTypeSetter = RequiredMethod(
                typeof(RewardGoods),
                $"set_{nameof(RewardGoods.RewardType)}",
                BindingFlags.Instance | BindingFlags.Public,
                [typeof(int)]);
            MethodInfo rewardGoodsTemplateIdSetter = RequiredMethod(
                typeof(RewardGoods),
                $"set_{nameof(RewardGoods.TemplateId)}",
                BindingFlags.Instance | BindingFlags.Public,
                [typeof(int)]);
            MethodInfo rewardGoodsCountSetter = RequiredMethod(
                typeof(RewardGoods),
                $"set_{nameof(RewardGoods.Count)}",
                BindingFlags.Instance | BindingFlags.Public,
                [typeof(int)]);

            List<IlInstruction> instructions = ReadIlInstructions(method).ToList();

            int changeGenderTimeIndex = FindCallIndex(instructions, changeGenderTimeSetter, startIndex: 0);
            if (changeGenderTimeIndex < 0)
                throw new InvalidDataException($"{name}: expected success path to set PlayerData.ChangeGenderTime.");

            int rewardInventoryIndex = FindMethodCallWithRecentConstants(instructions, inventoryDo, AscNet.Common.Database.Inventory.FreeGem, 50);
            if (rewardInventoryIndex < 0)
                throw new InvalidDataException($"{name}: expected first setup path to grant 50 Black Cards through Inventory.Do(Inventory.FreeGem, 50).");

            if (!HasConditionalBranchGuard(instructions, rewardInventoryIndex, rewardInventoryIndex))
                throw new InvalidDataException($"{name}: expected Black Card reward grant to be guarded by first gender setup.");

            if (!HasMethodCallStrictlyPositiveComparisonBefore(instructions, changeGenderTimeGetter, endIndex: rewardInventoryIndex))
                throw new InvalidDataException($"{name}: expected first setup reward guard to classify PlayerData.ChangeGenderTime <= 0 as incomplete setup.");

            int notifyItemPushIndex = FindGenericCallIndex(instructions, sendPush, typeof(NotifyItemDataList), rewardInventoryIndex + 1);
            if (notifyItemPushIndex < 0)
                throw new InvalidDataException($"{name}: expected first setup reward to push NotifyItemDataList.");

            int rewardGoodsListLoadIndex = FindFieldLoadIndex(instructions, responseRewardGoodsListField, notifyItemPushIndex + 1);
            if (rewardGoodsListLoadIndex < 0)
                throw new InvalidDataException($"{name}: expected first setup response to load ChangePlayerGenderResponse.RewardGoodsList.");

            int rewardGoodsAddIndex = FindCallIndex(instructions, rewardGoodsListAdd, rewardGoodsListLoadIndex + 1);
            if (rewardGoodsAddIndex < 0)
                throw new InvalidDataException($"{name}: expected first setup response to add a RewardGoods entry.");

            AssertSetterAssignedConstantBetween(instructions, rewardGoodsRewardTypeSetter, (int)RewardType.Item, rewardGoodsListLoadIndex, rewardGoodsAddIndex, $"{name} reward type");
            AssertSetterAssignedConstantBetween(instructions, rewardGoodsTemplateIdSetter, AscNet.Common.Database.Inventory.FreeGem, rewardGoodsListLoadIndex, rewardGoodsAddIndex, $"{name} reward item");
            AssertSetterAssignedConstantBetween(instructions, rewardGoodsCountSetter, 50, rewardGoodsListLoadIndex, rewardGoodsAddIndex, $"{name} reward count");

            int inventorySaveIndex = FindCallIndex(instructions, inventorySave, rewardGoodsAddIndex + 1);
            if (inventorySaveIndex < 0)
                throw new InvalidDataException($"{name}: expected first setup reward to persist Inventory.Save.");

            int playerSaveIndex = FindCallIndex(instructions, playerSave, inventorySaveIndex + 1);
            if (playerSaveIndex < 0)
                throw new InvalidDataException($"{name}: expected success path to save Player after first setup reward handling.");

            int finalResponseIndex = FindGenericCallIndex(instructions, sendResponse, typeof(ChangePlayerGenderResponse), playerSaveIndex + 1);
            if (finalResponseIndex < 0)
                throw new InvalidDataException($"{name}: expected saved success path to send ChangePlayerGenderResponse.");

            if (changeGenderTimeIndex >= playerSaveIndex)
                throw new InvalidDataException($"{name}: expected ChangeGenderTime to be set before Player.Save.");
            if (notifyItemPushIndex >= inventorySaveIndex)
                throw new InvalidDataException($"{name}: expected NotifyItemDataList push before Inventory.Save.");
            if (inventorySaveIndex >= finalResponseIndex)
                throw new InvalidDataException($"{name}: expected Inventory.Save before the success response.");
            if (playerSaveIndex >= finalResponseIndex)
                throw new InvalidDataException($"{name}: expected Player.Save before the success response.");
        }

        private static void AssertLiveGenderRefreshBeforeSuccessResponse(
            MethodInfo method,
            MethodInfo playerDataGetter,
            MethodInfo playerGenderGetter,
            MethodInfo playerGenderSetter,
            MethodInfo changeGenderTimeGetter,
            MethodInfo changeGenderTimeSetter,
            FieldInfo responseGenderField,
            FieldInfo responseChangeGenderTimeField,
            FieldInfo responseNextCanChangeTimeField,
            FieldInfo responsePlayerDataField,
            FieldInfo notifyGenderField,
            FieldInfo notifyChangeGenderTimeField,
            string name)
        {
            MethodInfo sendPush = RequiredGenericMethodDefinition(
                typeof(Session),
                nameof(Session.SendPush),
                BindingFlags.Instance | BindingFlags.Public,
                parameterCount: 1);
            MethodInfo sendResponse = RequiredGenericMethodDefinition(
                typeof(Session),
                nameof(Session.SendResponse),
                BindingFlags.Instance | BindingFlags.Public,
                parameterCount: 2);

            List<IlInstruction> instructions = ReadIlInstructions(method).ToList();

            int genderSetIndex = FindCallIndex(instructions, playerGenderSetter, startIndex: 0);
            if (genderSetIndex < 0)
                throw new InvalidDataException($"{name}: expected success path to set PlayerData.Gender.");

            int changeGenderTimeSetIndex = FindCallIndex(instructions, changeGenderTimeSetter, startIndex: genderSetIndex + 1);
            if (changeGenderTimeSetIndex < 0)
                throw new InvalidDataException($"{name}: expected success path to set PlayerData.ChangeGenderTime after PlayerData.Gender.");

            int updatedStateIndex = Math.Max(genderSetIndex, changeGenderTimeSetIndex);
            int notifyPushIndex = FindGenericCallIndex(instructions, sendPush, typeof(NotifyPlayerGender), updatedStateIndex + 1);
            if (notifyPushIndex < 0)
                throw new InvalidDataException($"{name}: expected updated gender state to be pushed through NotifyPlayerGender.");

            int successResponseIndex = FindGenericCallIndex(instructions, sendResponse, typeof(ChangePlayerGenderResponse), notifyPushIndex + 1);
            if (successResponseIndex < 0)
                throw new InvalidDataException($"{name}: expected NotifyPlayerGender to precede the success ChangePlayerGenderResponse.");

            int notifyGenderSetIndex = FindFieldAssignmentIndex(instructions, notifyGenderField, updatedStateIndex + 1, notifyPushIndex);
            if (notifyGenderSetIndex < 0)
                throw new InvalidDataException($"{name}: expected NotifyPlayerGender.Gender to be populated before SendPush.");
            AssertFieldAssignmentUsesRecentCall(instructions, notifyGenderSetIndex, playerGenderGetter, $"{name} notify gender source");

            int notifyChangeGenderTimeSetIndex = FindFieldAssignmentIndex(instructions, notifyChangeGenderTimeField, updatedStateIndex + 1, notifyPushIndex);
            if (notifyChangeGenderTimeSetIndex < 0)
                throw new InvalidDataException($"{name}: expected NotifyPlayerGender.ChangeGenderTime to be populated before SendPush.");
            AssertFieldAssignmentUsesRecentCall(instructions, notifyChangeGenderTimeSetIndex, changeGenderTimeGetter, $"{name} notify change-time source");

            int responseGenderSetIndex = FindFieldAssignmentIndex(instructions, responseGenderField, updatedStateIndex + 1, successResponseIndex);
            if (responseGenderSetIndex < 0)
                throw new InvalidDataException($"{name}: expected ChangePlayerGenderResponse.Gender to be populated before SendResponse.");
            AssertFieldAssignmentUsesRecentCall(instructions, responseGenderSetIndex, playerGenderGetter, $"{name} response gender source");

            int responseChangeGenderTimeSetIndex = FindFieldAssignmentIndex(instructions, responseChangeGenderTimeField, updatedStateIndex + 1, successResponseIndex);
            if (responseChangeGenderTimeSetIndex < 0)
                throw new InvalidDataException($"{name}: expected ChangePlayerGenderResponse.ChangeGenderTime to be populated before SendResponse.");
            AssertFieldAssignmentUsesRecentCall(instructions, responseChangeGenderTimeSetIndex, changeGenderTimeGetter, $"{name} response change-time source");

            int responseNextCanChangeTimeSetIndex = FindFieldAssignmentIndex(instructions, responseNextCanChangeTimeField, updatedStateIndex + 1, successResponseIndex);
            if (responseNextCanChangeTimeSetIndex < 0)
                throw new InvalidDataException($"{name}: expected ChangePlayerGenderResponse.NextCanChangeTime to be populated before SendResponse.");
            AssertFieldAssignmentUsesRecentCall(instructions, responseNextCanChangeTimeSetIndex, changeGenderTimeGetter, $"{name} response next-change-time source");

            int responsePlayerDataSetIndex = FindFieldAssignmentIndex(instructions, responsePlayerDataField, updatedStateIndex + 1, successResponseIndex);
            if (responsePlayerDataSetIndex < 0)
                throw new InvalidDataException($"{name}: expected ChangePlayerGenderResponse.PlayerData to be populated before SendResponse.");
            AssertFieldAssignmentUsesRecentCall(instructions, responsePlayerDataSetIndex, playerDataGetter, $"{name} response player-data source");

            if (notifyPushIndex >= successResponseIndex)
                throw new InvalidDataException($"{name}: expected NotifyPlayerGender push before the success response.");
        }

        private static bool HasGenderBoundBranch(List<IlInstruction> instructions, FieldInfo requestGenderField, int bound, int invalidCodeIndex, int invalidResponseIndex, bool lowerBound)
        {
            int firstCandidateIndex = Math.Max(0, invalidCodeIndex - 48);
            int invalidResponseOffset = instructions[invalidResponseIndex].Offset;

            for (int index = firstCandidateIndex; index < invalidCodeIndex; index++)
            {
                IlInstruction instruction = instructions[index];
                if (instruction.OpCode.FlowControl != FlowControl.Cond_Branch || instruction.Operand is not int targetOffset)
                    continue;
                if (!InstructionWindowLoadsFieldAndConstant(instructions, requestGenderField, bound, index, maxInstructionsBack: 8))
                    continue;

                bool targetReachesInvalidResponse = targetOffset > instruction.Offset && targetOffset <= invalidResponseOffset;
                bool targetSkipsInvalidResponse = targetOffset > invalidResponseOffset;

                if (lowerBound)
                {
                    if ((targetReachesInvalidResponse && IsLessThanBranch(instruction.OpCode))
                        || (targetSkipsInvalidResponse && IsGreaterThanOrEqualBranch(instruction.OpCode)))
                        return true;
                }
                else
                {
                    if ((targetReachesInvalidResponse && IsGreaterThanBranch(instruction.OpCode))
                        || (targetSkipsInvalidResponse && IsLessThanOrEqualBranch(instruction.OpCode)))
                        return true;
                }
            }

            return false;
        }

        private static bool HasNormalizedInclusiveRangeGuard(List<IlInstruction> instructions, FieldInfo requestGenderField, int minimum, int maximum, int invalidCodeIndex, int invalidResponseIndex)
        {
            int firstCandidateIndex = Math.Max(0, invalidCodeIndex - 48);
            int invalidResponseOffset = instructions[invalidResponseIndex].Offset;
            int normalizedMaximum = maximum - minimum;

            for (int index = firstCandidateIndex; index < invalidCodeIndex; index++)
            {
                IlInstruction instruction = instructions[index];
                if (instruction.OpCode.FlowControl != FlowControl.Cond_Branch || instruction.Operand is not int targetOffset)
                    continue;
                if (!InstructionWindowLoadsFieldAndConstant(instructions, requestGenderField, minimum, index, maxInstructionsBack: 10))
                    continue;
                if (!InstructionWindowHasConstant(instructions, normalizedMaximum, index, maxInstructionsBack: 10))
                    continue;
                if (!InstructionWindowHasOpCode(instructions, OpCodes.Sub, index, maxInstructionsBack: 10))
                    continue;

                bool targetReachesInvalidResponse = targetOffset > instruction.Offset && targetOffset <= invalidResponseOffset;
                bool targetSkipsInvalidResponse = targetOffset > invalidResponseOffset;
                if ((targetSkipsInvalidResponse && IsLessThanOrEqualUnsignedBranch(instruction.OpCode))
                    || (targetReachesInvalidResponse && IsGreaterThanUnsignedBranch(instruction.OpCode)))
                    return true;
            }

            return false;
        }

        private static bool InstructionWindowLoadsFieldAndConstant(List<IlInstruction> instructions, FieldInfo field, int expectedValue, int endIndex, int maxInstructionsBack)
        {
            bool loadedField = false;
            bool loadedConstant = false;
            int firstIndex = Math.Max(0, endIndex - maxInstructionsBack);

            for (int index = firstIndex; index < endIndex; index++)
            {
                if (instructions[index].Operand is FieldInfo loadedFieldInfo && FieldsMatch(loadedFieldInfo, field))
                    loadedField = true;
                if (LdcI4Value(instructions[index]) == expectedValue)
                    loadedConstant = true;
            }

            return loadedField && loadedConstant;
        }

        private static bool InstructionWindowHasConstant(List<IlInstruction> instructions, int expectedValue, int endIndex, int maxInstructionsBack)
        {
            int firstIndex = Math.Max(0, endIndex - maxInstructionsBack);
            for (int index = firstIndex; index < endIndex; index++)
            {
                if (LdcI4Value(instructions[index]) == expectedValue)
                    return true;
            }

            return false;
        }

        private static bool InstructionWindowHasOpCode(List<IlInstruction> instructions, OpCode opCode, int endIndex, int maxInstructionsBack)
        {
            int firstIndex = Math.Max(0, endIndex - maxInstructionsBack);
            for (int index = firstIndex; index < endIndex; index++)
            {
                if (instructions[index].OpCode == opCode)
                    return true;
            }

            return false;
        }

        private static bool HasMethodCallComparedToConstantBefore(List<IlInstruction> instructions, MethodInfo method, int expectedValue, int endIndex)
        {
            for (int index = 0; index < endIndex; index++)
            {
                if (instructions[index].Operand is not MethodBase calledMethod || !MethodsMatch(calledMethod, method))
                    continue;

                bool loadedConstant = false;
                bool comparedOrBranched = false;
                int lastIndex = Math.Min(endIndex, index + 10);
                for (int scanIndex = index + 1; scanIndex < lastIndex; scanIndex++)
                {
                    if (LdcI4Value(instructions[scanIndex]) == expectedValue)
                        loadedConstant = true;
                    if (IsComparisonOrConditionalBranch(instructions[scanIndex].OpCode))
                        comparedOrBranched = true;
                }

                if (loadedConstant && comparedOrBranched)
                    return true;
            }

            return false;
        }

        private static bool HasMethodCallStrictlyPositiveComparisonBefore(List<IlInstruction> instructions, MethodInfo method, int endIndex)
        {
            for (int index = 0; index < endIndex; index++)
            {
                if (instructions[index].Operand is not MethodBase calledMethod || !MethodsMatch(calledMethod, method))
                    continue;

                bool loadedZero = false;
                int lastIndex = Math.Min(endIndex, index + 12);
                for (int scanIndex = index + 1; scanIndex < lastIndex; scanIndex++)
                {
                    if (LdcI4Value(instructions[scanIndex]) == 0)
                    {
                        loadedZero = true;
                        continue;
                    }

                    if (!loadedZero)
                        continue;

                    OpCode opCode = instructions[scanIndex].OpCode;

                    if (opCode == OpCodes.Cgt
                        || IsGreaterThanBranch(opCode)
                        || IsLessThanOrEqualBranch(opCode))
                        return true;
                }
            }

            return false;
        }

        private static bool HasSameGenderComparisonBefore(List<IlInstruction> instructions, MethodInfo playerGenderGetter, FieldInfo requestGenderField, int endIndex)
        {
            for (int index = 0; index < endIndex; index++)
            {
                if (instructions[index].Operand is not MethodBase calledMethod || !MethodsMatch(calledMethod, playerGenderGetter))
                    continue;

                bool loadedRequestGender = false;
                bool comparedOrBranched = false;
                int lastIndex = Math.Min(endIndex, index + 20);
                for (int scanIndex = index + 1; scanIndex < lastIndex; scanIndex++)
                {
                    if (instructions[scanIndex].Operand is FieldInfo loadedField && FieldsMatch(loadedField, requestGenderField))
                        loadedRequestGender = true;
                    if (IsComparisonOrConditionalBranch(instructions[scanIndex].OpCode))
                        comparedOrBranched = true;
                }

                if (loadedRequestGender && comparedOrBranched)
                    return true;
            }

            return false;
        }

        private static int FindCallIndex(List<IlInstruction> instructions, MethodInfo target, int startIndex)
        {
            for (int index = startIndex; index < instructions.Count; index++)
            {
                if (instructions[index].Operand is MethodBase calledMethod && MethodsMatch(calledMethod, target))
                    return index;
            }

            return -1;
        }

        private static int FindMethodCallWithRecentConstants(List<IlInstruction> instructions, MethodInfo target, params int[] expectedValues)
        {
            for (int index = 0; index < instructions.Count; index++)
            {
                if (instructions[index].Operand is not MethodBase calledMethod || !MethodsMatch(calledMethod, target))
                    continue;

                bool foundAllValues = true;
                foreach (int expectedValue in expectedValues)
                {
                    if (!InstructionWindowHasConstant(instructions, expectedValue, index, maxInstructionsBack: 12))
                    {
                        foundAllValues = false;
                        break;
                    }
                }

                if (foundAllValues)
                    return index;
            }

            return -1;
        }

        private static int FindFieldLoadIndex(List<IlInstruction> instructions, FieldInfo field, int startIndex)
        {
            for (int index = startIndex; index < instructions.Count; index++)
            {
                IlInstruction instruction = instructions[index];
                if ((instruction.OpCode == OpCodes.Ldfld || instruction.OpCode == OpCodes.Ldflda)
                    && instruction.Operand is FieldInfo loadedField
                    && FieldsMatch(loadedField, field))
                    return index;
            }

            return -1;
        }

        private static int FindFieldAssignmentIndex(List<IlInstruction> instructions, FieldInfo field, int startIndex, int endIndex)
        {
            int firstIndex = Math.Max(0, startIndex);
            int lastIndex = Math.Min(instructions.Count - 1, endIndex);
            for (int index = firstIndex; index <= lastIndex; index++)
            {
                IlInstruction instruction = instructions[index];
                if (instruction.OpCode == OpCodes.Stfld
                    && instruction.Operand is FieldInfo assignedField
                    && FieldsMatch(assignedField, field))
                    return index;
            }

            return -1;
        }

        private static void AssertFieldAssignmentUsesRecentCall(List<IlInstruction> instructions, int assignmentIndex, MethodInfo sourceMethod, string name)
        {
            for (int previousIndex = assignmentIndex - 1; previousIndex >= 0 && previousIndex >= assignmentIndex - 12; previousIndex--)
            {
                if (instructions[previousIndex].Operand is MethodBase calledMethod && MethodsMatch(calledMethod, sourceMethod))
                    return;
            }

            throw new InvalidDataException($"{name}: expected assignment to use {sourceMethod.DeclaringType?.FullName}.{sourceMethod.Name}.");
        }

        private static void AssertSetterAssignedConstantBetween(List<IlInstruction> instructions, MethodInfo setter, int expectedValue, int startIndex, int endIndex, string name)
        {
            for (int index = startIndex; index <= endIndex; index++)
            {
                if (instructions[index].Operand is not MethodBase calledMethod || !MethodsMatch(calledMethod, setter))
                    continue;
                if (InstructionWindowHasConstant(instructions, expectedValue, index, maxInstructionsBack: 8))
                    return;
            }

            throw new InvalidDataException($"{name}: expected {setter.DeclaringType?.FullName}.{setter.Name} to be assigned {expectedValue}.");
        }

        private static bool IsComparisonOrConditionalBranch(OpCode opCode)
        {
            return opCode.FlowControl == FlowControl.Cond_Branch
                || opCode == OpCodes.Ceq
                || opCode == OpCodes.Cgt
                || opCode == OpCodes.Cgt_Un
                || opCode == OpCodes.Clt
                || opCode == OpCodes.Clt_Un;
        }

        private static bool IsLessThanBranch(OpCode opCode)
        {
            return opCode == OpCodes.Blt || opCode == OpCodes.Blt_S;
        }

        private static bool IsGreaterThanBranch(OpCode opCode)
        {
            return opCode == OpCodes.Bgt || opCode == OpCodes.Bgt_S;
        }

        private static bool IsGreaterThanUnsignedBranch(OpCode opCode)
        {
            return opCode == OpCodes.Bgt_Un || opCode == OpCodes.Bgt_Un_S;
        }

        private static bool IsLessThanOrEqualBranch(OpCode opCode)
        {
            return opCode == OpCodes.Ble || opCode == OpCodes.Ble_S;
        }

        private static bool IsLessThanOrEqualUnsignedBranch(OpCode opCode)
        {
            return opCode == OpCodes.Ble_Un || opCode == OpCodes.Ble_Un_S;
        }

        private static bool IsGreaterThanOrEqualBranch(OpCode opCode)
        {
            return opCode == OpCodes.Bge || opCode == OpCodes.Bge_S;
        }

        private static int FindFieldAssignmentIndex(List<IlInstruction> instructions, FieldInfo field, int expectedValue)
        {
            for (int index = 0; index < instructions.Count; index++)
            {
                IlInstruction instruction = instructions[index];
                if (instruction.OpCode != OpCodes.Stfld || instruction.Operand is not FieldInfo assignedField || !FieldsMatch(assignedField, field))
                    continue;

                for (int previousIndex = index - 1; previousIndex >= 0 && previousIndex >= index - 6; previousIndex--)
                {
                    if (LdcI4Value(instructions[previousIndex]) == expectedValue)
                        return index;
                }
            }

            return -1;
        }

        private static int FindGenericCallIndex(List<IlInstruction> instructions, MethodInfo genericMethodDefinition, Type genericArgument, int startIndex)
        {
            for (int index = startIndex; index < instructions.Count; index++)
            {
                if (instructions[index].Operand is MethodBase calledMethod && GenericMethodsMatch(calledMethod, genericMethodDefinition, genericArgument))
                    return index;
            }

            return -1;
        }

        private static int FindFirstInventoryMutationIndex(List<IlInstruction> instructions, MethodInfo inventoryDo, MethodInfo inventorySave)
        {
            for (int index = 0; index < instructions.Count; index++)
            {
                if (instructions[index].Operand is not MethodInfo calledMethod)
                    continue;

                if (MethodsMatch(calledMethod, inventoryDo)
                    || MethodsMatch(calledMethod, inventorySave)
                    || CallsTargetTransitively(calledMethod, inventorySave, []))
                    return index;
            }

            return -1;
        }

        private static bool HasConditionalBranchGuard(List<IlInstruction> instructions, int guardedStartIndex, int guardedEndIndex)
        {
            const int maxInstructionsToInspect = 16;
            int firstCandidateIndex = Math.Max(0, guardedStartIndex - maxInstructionsToInspect);
            int guardedStartOffset = instructions[guardedStartIndex].Offset;
            int guardedEndOffset = instructions[guardedEndIndex].Offset;

            for (int index = guardedStartIndex - 1; index >= firstCandidateIndex; index--)
            {
                IlInstruction instruction = instructions[index];
                if (instruction.OpCode.FlowControl != FlowControl.Cond_Branch || instruction.Operand is not int targetOffset)
                    continue;

                if ((targetOffset >= guardedStartOffset && targetOffset <= guardedEndOffset) || targetOffset > guardedEndOffset)
                    return true;
            }

            return false;
        }

        private static bool PathExits(List<IlInstruction> instructions, int startIndex)
        {
            IlInstruction? nextMeaningfulInstruction = NextMeaningfulInstruction(instructions, startIndex);
            if (nextMeaningfulInstruction is null)
                return false;
            if (nextMeaningfulInstruction.Value.OpCode.FlowControl == FlowControl.Return)
                return true;
            if (nextMeaningfulInstruction.Value.OpCode.FlowControl != FlowControl.Branch || nextMeaningfulInstruction.Value.Operand is not int targetOffset)
                return false;

            int targetIndex = instructions.FindIndex(instruction => instruction.Offset == targetOffset);
            if (targetIndex < 0)
                return false;

            IlInstruction? branchTargetInstruction = NextMeaningfulInstruction(instructions, targetIndex);
            return branchTargetInstruction?.OpCode.FlowControl == FlowControl.Return;
        }

        private static IlInstruction? NextMeaningfulInstruction(List<IlInstruction> instructions, int startIndex)
        {
            for (int index = startIndex; index < instructions.Count; index++)
            {
                if (instructions[index].OpCode != OpCodes.Nop)
                    return instructions[index];
            }

            return null;
        }

        private static IEnumerable<IlInstruction> ReadIlInstructions(MethodInfo method)
        {
            byte[] il = method.GetMethodBody()?.GetILAsByteArray()
                ?? throw new InvalidDataException($"{method.DeclaringType?.FullName}.{method.Name}: method body is unavailable.");
            Module module = method.Module;
            Type[] typeArguments = method.DeclaringType?.GetGenericArguments() ?? Type.EmptyTypes;
            Type[] methodArguments = method.GetGenericArguments();

            for (int offset = 0; offset < il.Length;)
            {
                int instructionOffset = offset;
                OpCode opCode = ReadOpCode(il, ref offset);
                object? operand = ReadOperand(il, ref offset, opCode, module, typeArguments, methodArguments);
                yield return new IlInstruction(instructionOffset, opCode, operand);
            }
        }

        private static object? ReadOperand(byte[] il, ref int offset, OpCode opCode, Module module, Type[] typeArguments, Type[] methodArguments)
        {
            int operandOffset = offset;
            switch (opCode.OperandType)
            {
                case OperandType.InlineNone:
                    return null;
                case OperandType.ShortInlineI:
                    offset += 1;
                    return (int)(sbyte)il[operandOffset];
                case OperandType.InlineI:
                    offset += 4;
                    return BitConverter.ToInt32(il, operandOffset);
                case OperandType.ShortInlineBrTarget:
                    offset += 1;
                    return offset + (sbyte)il[operandOffset];
                case OperandType.InlineBrTarget:
                    offset += 4;
                    return offset + BitConverter.ToInt32(il, operandOffset);
                case OperandType.InlineMethod:
                    offset += 4;
                    return ResolveToken(module, BitConverter.ToInt32(il, operandOffset), typeArguments, methodArguments);
                case OperandType.InlineField:
                    offset += 4;
                    return ResolveField(module, BitConverter.ToInt32(il, operandOffset), typeArguments, methodArguments);
                default:
                    offset += OperandSize(opCode.OperandType, il, operandOffset);
                    return null;
            }
        }

        private static MemberInfo? ResolveToken(Module module, int metadataToken, Type[] typeArguments, Type[] methodArguments)
        {
            try
            {
                return module.ResolveMethod(metadataToken, typeArguments, methodArguments);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        private static FieldInfo? ResolveField(Module module, int metadataToken, Type[] typeArguments, Type[] methodArguments)
        {
            try
            {
                return module.ResolveField(metadataToken, typeArguments, methodArguments);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        private static int? LdcI4Value(IlInstruction instruction)
        {
            if (instruction.OpCode == OpCodes.Ldc_I4_M1)
                return -1;
            if (instruction.OpCode == OpCodes.Ldc_I4_0)
                return 0;
            if (instruction.OpCode == OpCodes.Ldc_I4_1)
                return 1;
            if (instruction.OpCode == OpCodes.Ldc_I4_2)
                return 2;
            if (instruction.OpCode == OpCodes.Ldc_I4_3)
                return 3;
            if (instruction.OpCode == OpCodes.Ldc_I4_4)
                return 4;
            if (instruction.OpCode == OpCodes.Ldc_I4_5)
                return 5;
            if (instruction.OpCode == OpCodes.Ldc_I4_6)
                return 6;
            if (instruction.OpCode == OpCodes.Ldc_I4_7)
                return 7;
            if (instruction.OpCode == OpCodes.Ldc_I4_8)
                return 8;
            if (instruction.OpCode is { } opCode && (opCode == OpCodes.Ldc_I4 || opCode == OpCodes.Ldc_I4_S) && instruction.Operand is int value)
                return value;

            return null;
        }

        private static bool FieldsMatch(FieldInfo candidate, FieldInfo target)
        {
            return candidate.Module == target.Module && candidate.MetadataToken == target.MetadataToken;
        }

        private readonly record struct IlInstruction(int Offset, OpCode OpCode, object? Operand);

        private static bool NextMeaningfulOpCodeIsConditionalBranch(byte[] il, int offset)
        {
            const int maxInstructionsToInspect = 8;
            int inspectedInstructions = 0;

            while (offset < il.Length && inspectedInstructions < maxInstructionsToInspect)
            {
                OpCode opCode = ReadOpCode(il, ref offset);
                offset += OperandSize(opCode.OperandType, il, offset);
                if (opCode == OpCodes.Nop)
                    continue;

                inspectedInstructions++;
                if (opCode.FlowControl == FlowControl.Cond_Branch)
                    return true;
                if (opCode.OperandType == OperandType.InlineMethod)
                    return false;
            }

            return false;
        }

        private static IEnumerable<MethodBase> CalledMethods(MethodInfo method)
        {
            byte[] il = method.GetMethodBody()?.GetILAsByteArray()
                ?? throw new InvalidDataException($"{method.DeclaringType?.FullName}.{method.Name}: method body is unavailable.");
            Module module = method.Module;
            Type[] typeArguments = method.DeclaringType?.GetGenericArguments() ?? Type.EmptyTypes;
            Type[] methodArguments = method.GetGenericArguments();

            for (int offset = 0; offset < il.Length;)
            {
                OpCode opCode = ReadOpCode(il, ref offset);
                if (opCode.OperandType == OperandType.InlineMethod)
                {
                    int metadataToken = BitConverter.ToInt32(il, offset);
                    offset += 4;

                    MethodBase? calledMethod;
                    try
                    {
                        calledMethod = module.ResolveMethod(metadataToken, typeArguments, methodArguments);
                    }
                    catch (ArgumentException)
                    {
                        continue;
                    }

                    if (calledMethod is not null)
                        yield return calledMethod;
                }
                else
                {
                    offset += OperandSize(opCode.OperandType, il, offset);
                }
            }
        }

        private static OpCode ReadOpCode(byte[] il, ref int offset)
        {
            byte value = il[offset++];
            if (value != 0xFE)
                return SingleByteOpCodes[value];

            return MultiByteOpCodes[il[offset++]];
        }

        private static int OperandSize(OperandType operandType, byte[] il, int offset)
        {
            return operandType switch
            {
                OperandType.InlineNone => 0,
                OperandType.ShortInlineBrTarget or OperandType.ShortInlineI or OperandType.ShortInlineVar => 1,
                OperandType.InlineVar => 2,
                OperandType.InlineBrTarget or OperandType.InlineField or OperandType.InlineI or OperandType.InlineSig or OperandType.InlineString or OperandType.InlineTok or OperandType.InlineType or OperandType.ShortInlineR => 4,
                OperandType.InlineI8 or OperandType.InlineR => 8,
                OperandType.InlineSwitch => 4 + (BitConverter.ToInt32(il, offset) * 4),
                _ => throw new InvalidDataException($"Unsupported IL operand type {operandType}.")
            };
        }

        private static bool GenericMethodsMatch(MethodBase candidate, MethodInfo genericMethodDefinition, Type genericArgument)
        {
            if (candidate is not MethodInfo candidateMethod || !candidateMethod.IsGenericMethod)
                return false;

            MethodInfo candidateDefinition = candidateMethod.IsGenericMethodDefinition
                ? candidateMethod
                : candidateMethod.GetGenericMethodDefinition();
            if (candidateDefinition.Module != genericMethodDefinition.Module || candidateDefinition.MetadataToken != genericMethodDefinition.MetadataToken)
                return false;

            Type[] genericArguments = candidateMethod.GetGenericArguments();
            return genericArguments.Length == 1 && genericArguments[0] == genericArgument;
        }

        private static string FormatGenericMethod(MethodInfo genericMethodDefinition, Type genericArgument)
        {
            return $"{genericMethodDefinition.DeclaringType?.FullName}.{genericMethodDefinition.Name}<{genericArgument.FullName}>";
        }

        private static bool MethodsMatch(MethodBase candidate, MethodInfo target)
        {
            return candidate.Module == target.Module && candidate.MetadataToken == target.MetadataToken;
        }

        private static string MethodKey(MethodBase method)
        {
            return $"{method.Module.ModuleVersionId:N}:{method.MetadataToken}";
        }

        private static readonly OpCode[] SingleByteOpCodes = BuildOpCodeTable(multiByte: false);
        private static readonly OpCode[] MultiByteOpCodes = BuildOpCodeTable(multiByte: true);

        private static OpCode[] BuildOpCodeTable(bool multiByte)
        {
            OpCode[] table = new OpCode[0x100];
            foreach (FieldInfo field in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.GetValue(null) is not OpCode opCode)
                    continue;

                ushort value = (ushort)opCode.Value;
                if (multiByte)
                {
                    if ((value & 0xFF00) == 0xFE00)
                        table[value & 0xFF] = opCode;
                }
                else if (value < 0x100)
                {
                    table[value] = opCode;
                }
            }

            return table;
        }

        private static void ValidateCurrentClientNoticeFixtures()
        {
            JObject loginNotice = JObject.Parse(File.ReadAllText(ResourcePath("Configs", "Notices", "4.5.0", "LoginNotice.json")));
            AssertEqual("6a194a09f1b4a1288a8994ca", loginNotice.Value<string>("Id"), "LoginNotice Id");
            AssertEqual(1780042249545, loginNotice.Value<long>("ModifyTime"), "LoginNotice ModifyTime");
            AssertEqual("client/notice/html/6a1949e1f1b4a1288a8994c9.html", loginNotice.Value<string>("HtmlUrl"), "LoginNotice HtmlUrl");

            JObject scrollTextNotice = JObject.Parse(File.ReadAllText(ResourcePath("Configs", "Notices", "4.5.0", "ScrollTextNotice.json")));
            AssertEqual("6a194a9df1b4a1288a8994cb", scrollTextNotice.Value<string>("Id"), "ScrollTextNotice Id");
            AssertEqual(300, scrollTextNotice.Value<int>("ScrollInterval"), "ScrollTextNotice ScrollInterval");
            if (!scrollTextNotice.Value<string>("Content")!.Contains("Homecoming Voyage", StringComparison.Ordinal))
                throw new InvalidDataException("ScrollTextNotice content is not the current 4.5.0 notice.");

            JObject scrollPicNotice = JObject.Parse(File.ReadAllText(ResourcePath("Configs", "Notices", "4.5.0", "ScrollPicNotice.json")));
            AssertEqual("6a1e16a2f1b4a13fd8bf490e", scrollPicNotice.Value<string>("Id"), "ScrollPicNotice Id");
            AssertEqual(10, scrollPicNotice.Value<JArray>("Content")!.Count, "ScrollPicNotice content count");

            JArray gameNotices = JArray.Parse(File.ReadAllText(ResourcePath("Configs", "Notices", "4.5.0", "GameNotice.json")));
            AssertEqual(16, gameNotices.Count, "GameNotice count");
            AssertEqual("EDEN BROADCAST", gameNotices[0]!.Value<string>("Title"), "GameNotice first title");
        }

        private static async Task ValidateCurrentClientNoticeEndpoints()
        {
            await using WebApplication app = CreateConfigControllerTestApp();
            await app.StartAsync();

            try
            {
                using HttpClient client = new()
                {
                    BaseAddress = new Uri(BoundAddress(app))
                };

                string[] endpoints =
                [
                    "/prod/client/notice/config/prod-encdn-tx/com.kurogame.pc.punishing.grayraven.en/4.5.0/SecondMenuNotice.json",
                    "/prod/client/notice/config/prod-encdn-tx/com.kurogame.pc.punishing.grayraven.en/4.5.0/PopUpPicNotice.json",
                ];

                foreach (string endpoint in endpoints)
                {
                    using HttpResponseMessage response = await client.GetAsync(endpoint);
                    string body = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new InvalidDataException($"{endpoint}: expected HTTP 200, got {(int)response.StatusCode} {response.StatusCode}. Body: {body}");

                    AssertCurrentClientNoticePayload(JObject.Parse(body), endpoint);
                }
            }
            finally
            {
                await app.StopAsync();
            }
        }

        private static void AssertCurrentClientNoticePayload(JObject payload, string endpoint)
        {
            _ = RequiredNonEmptyString(payload, "Id", endpoint);
            _ = RequiredValue<long>(payload, "ModifyTime", JTokenType.Integer, endpoint);
            _ = RequiredToken(payload, "Content", JTokenType.Array, endpoint);
            _ = RequiredToken(payload, "LoginPlatformList", JTokenType.Array, endpoint);
        }

        private static void ValidateSteamClientConfig()
        {
            Type configController = Type.GetType("AscNet.SDKServer.Controllers.ConfigController, AscNet.SDKServer", throwOnError: true)!;

            MethodInfo getPackageConfig = configController.GetMethod("GetPackageConfig", BindingFlags.NonPublic | BindingFlags.Static)!;
            object packageConfig = getPackageConfig.Invoke(null, ["com.kurogame.pc.punishing.grayraven.en", true])!;
            AssertEqual("http://prod-encdn-tx.kurogame.net/prod", packageConfig.GetType().GetField("Item1")!.GetValue(packageConfig), "Steam PrimaryCdns");
            AssertEqual("http://prod-encdn-aliyun.kurogame.net/prod", packageConfig.GetType().GetField("Item2")!.GetValue(packageConfig), "Steam SecondaryCdns");
            AssertEqual(205, packageConfig.GetType().GetField("Item3")!.GetValue(packageConfig), "Steam Channel");

            MethodInfo addCurrentClientConfig = configController.GetMethod("AddCurrentClientConfig", BindingFlags.NonPublic | BindingFlags.Static)!;
            List<RemoteConfig> remoteConfigs = new();
            ServerVersionConfig versionConfig = new()
            {
                DocumentVersion = "4.5.12",
                LaunchModuleVersion = "4.5.12",
                IndexMd5 = "c5d4baac85a6e37b8109ea43dc045d31",
                IndexSha1 = "23aa5943c6b89d62ed6c1acea573e6ac2970b4bf",
                LaunchIndexSha1 = "b4ae904215964fe6dfc2d3c5f04bfd1ccf53b659"
            };
            addCurrentClientConfig.Invoke(null, [remoteConfigs, "com.kurogame.pc.punishing.grayraven.en", "4.5.0", versionConfig, "http://127.0.0.1:8080"]);

            AssertEqual("205", ConfigValue(remoteConfigs, "Channel"), "Steam Channel config");
            AssertEqual("4.5.12", ConfigValue(remoteConfigs, "DocumentVersion"), "Steam DocumentVersion config");
            AssertEqual("4.5.12", ConfigValue(remoteConfigs, "LaunchModuleVersion"), "Steam LaunchModuleVersion config");
            AssertEqual("http://127.0.0.1:8080/api/XPay/KuroPayResult", ConfigValue(remoteConfigs, "KuroPayCallbackUrl"), "KuroPayCallbackUrl");
            AssertEqual("http://127.0.0.1:8080/api/XPay/KuroPayResult", ConfigValue(remoteConfigs, "PcPayCallbackUrl"), "PcPayCallbackUrl");
        }

        private const string KuroSdkDummyEmail = "krsdk-test@ascnet.local";
        private const string KuroSdkDummyToken = "krsdk-local-token";
        private const string KuroSdkDummySteamId = "76561198000000000";
        private const string KuroSdkDummyCuid = "steam-76561198000000000";
        private const string KuroSdkDummyUsername = "SteamUser76561198000000000";
        private const int KuroSdkDummyLoginType = 37;
        private const string KuroSdkDummyMark = "ascnet-steam-login-mark";

        private static async Task ValidateKuroSdkCompatibilityEndpoints()
        {
            string? previousPublicHttpOrigin = Environment.GetEnvironmentVariable("ASCNET_PUBLIC_HTTP_ORIGIN");
            Environment.SetEnvironmentVariable("ASCNET_PUBLIC_HTTP_ORIGIN", null);
            await using WebApplication app = CreateKuroSdkTestApp();
            await app.StartAsync();

            try
            {
                using HttpClient client = new()
                {
                    BaseAddress = new Uri(BoundAddress(app))
                };

                KuroSdkEndpointContract[] endpoints =
                [
                    new("/sdkcom/v2/login/emailPwd.lg", AssertKuroSdkLoginData),
                    new("/sdkcom/v2/login/third/steam.lg", AssertKuroSdkLoginData),
                    new("/sdkcom/v2/login/auto.lg", AssertKuroSdkLoginData),
                    new("/sdkcom/v2/login/real-name/login.lg", AssertKuroSdkLoginData),
                    new("/sdkcom/v2/login/preambleCode.lg", AssertKuroSdkLoginData),
                    new("/sdkcom/v2/auth/getToken.lg", AssertKuroSdkAccessTokenData),
                    new("/sdkcom/v2/user/oauth/code/generate.lg", AssertKuroSdkOauthCodeData),
                    new("/sdkcom/v2/user/game/role.lg", AssertKuroSdkEmptyData),
                    new("/sdkcom/v2/heartbeat/tokenCheck.lg", AssertKuroSdkEmptyData),
                    new("/sdkcom/v2/bind/device/status.lg", AssertKuroSdkEmptyData),
                    new("/sdkcom/v2/real-name-info/check.lg", AssertKuroSdkRealNameCheckData),
                ];

                foreach (KuroSdkEndpointContract endpoint in endpoints)
                {
                    using HttpRequestMessage request = new(HttpMethod.Post, $"{endpoint.Path}?{KuroSdkHandoffQuery()}")
                    {
                        Content = new StringContent("{}", Encoding.UTF8, "application/json")
                    };
                    using HttpResponseMessage response = await client.SendAsync(request);
                    string body = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new InvalidDataException($"{endpoint.Path}: expected HTTP 200, got {(int)response.StatusCode} {response.StatusCode}. Body: {body}");

                    JObject payload = JObject.Parse(body);
                    AssertKuroSdkSuccessEnvelope(payload, endpoint.Path);
                    endpoint.AssertData(RequiredObject(payload, "data", endpoint.Path), endpoint.Path);
                }

                using HttpRequestMessage formLoginRequest = new(HttpMethod.Post, "/sdkcom/v2/login/third/steam.lg")
                {
                    Content = new FormUrlEncodedContent(KuroSdkHandoffFields())
                };
                using HttpResponseMessage formLoginResponse = await client.SendAsync(formLoginRequest);
                string formLoginBody = await formLoginResponse.Content.ReadAsStringAsync();

                if (formLoginResponse.StatusCode != HttpStatusCode.OK)
                    throw new InvalidDataException($"/sdkcom/v2/login/third/steam.lg form: expected HTTP 200, got {(int)formLoginResponse.StatusCode} {formLoginResponse.StatusCode}. Body: {formLoginBody}");

                JObject formLoginPayload = JObject.Parse(formLoginBody);
                AssertKuroSdkSuccessEnvelope(formLoginPayload, "/sdkcom/v2/login/third/steam.lg form");
                AssertKuroSdkLoginData(RequiredObject(formLoginPayload, "data", "/sdkcom/v2/login/third/steam.lg form"), "/sdkcom/v2/login/third/steam.lg form");

                using HttpRequestMessage markRequest = new(HttpMethod.Post, $"/sdkcom/v2/login/third/pc/mark.lg?{KuroSdkHandoffQuery(mark: KuroSdkDummyMark)}")
                {
                    Content = new StringContent("{}", Encoding.UTF8, "application/json")
                };
                using HttpResponseMessage markResponse = await client.SendAsync(markRequest);
                string markBody = await markResponse.Content.ReadAsStringAsync();

                if (markResponse.StatusCode != HttpStatusCode.OK)
                    throw new InvalidDataException($"/sdkcom/v2/login/third/pc/mark.lg: expected HTTP 200, got {(int)markResponse.StatusCode} {markResponse.StatusCode}. Body: {markBody}");

                JObject markPayload = JObject.Parse(markBody);
                AssertKuroSdkSuccessEnvelope(markPayload, "/sdkcom/v2/login/third/pc/mark.lg");
                AssertKuroSdkThirdLoginMarkData(RequiredObject(markPayload, "data", "/sdkcom/v2/login/third/pc/mark.lg"), "/sdkcom/v2/login/third/pc/mark.lg");

                using HttpRequestMessage browserRequest = new(HttpMethod.Post, $"/sdkcom/v2/login/third/pc/browser.lg?{KuroSdkHandoffQuery(mark: KuroSdkDummyMark)}")
                {
                    Content = new StringContent("{}", Encoding.UTF8, "application/json")
                };
                using HttpResponseMessage browserResponse = await client.SendAsync(browserRequest);
                string browserBody = await browserResponse.Content.ReadAsStringAsync();

                if (browserResponse.StatusCode != HttpStatusCode.OK)
                    throw new InvalidDataException($"/sdkcom/v2/login/third/pc/browser.lg: expected HTTP 200, got {(int)browserResponse.StatusCode} {browserResponse.StatusCode}. Body: {browserBody}");

                JObject browserPayload = JObject.Parse(browserBody);
                AssertKuroSdkSuccessCodeAndMessage(browserPayload, "/sdkcom/v2/login/third/pc/browser.lg");
                string browserUrl = RequiredValue<string>(browserPayload, "url", JTokenType.String, "/sdkcom/v2/login/third/pc/browser.lg");
                AssertEqual(browserUrl, RequiredValue<string>(browserPayload, "data", JTokenType.String, "/sdkcom/v2/login/third/pc/browser.lg"), "/sdkcom/v2/login/third/pc/browser.lg data");
                AssertKuroSdkLocalLoginUrl(browserUrl, "/sdkcom/v2/login/third/pc/browser.lg url", KuroSdkDummyMark, KuroSdkDummyLoginType.ToString());

                string[] systemConfigEndpoints =
                [
                    "/sdkcom/v2/sys/europe/config.lg",
                    "/sdkcom/v2/sys/conf.lg",
                ];

                foreach (string systemConfigEndpoint in systemConfigEndpoints)
                {
                    using HttpResponseMessage systemConfigResponse = await client.GetAsync(systemConfigEndpoint);
                    string systemConfigBody = await systemConfigResponse.Content.ReadAsStringAsync();

                    if (systemConfigResponse.StatusCode != HttpStatusCode.OK)
                        throw new InvalidDataException($"{systemConfigEndpoint}: expected HTTP 200, got {(int)systemConfigResponse.StatusCode} {systemConfigResponse.StatusCode}. Body: {systemConfigBody}");

                    JObject payload = JObject.Parse(systemConfigBody);
                    AssertKuroSdkSuccessEnvelope(payload, systemConfigEndpoint);
                    AssertKuroSdkSystemConfigData(RequiredObject(payload, "data", systemConfigEndpoint), systemConfigEndpoint);
                }

                using HttpResponseMessage playerConfigResponse = await client.GetAsync("/sdkcom/v2/sys/player-config.json");
                string playerConfigBody = await playerConfigResponse.Content.ReadAsStringAsync();

                if (playerConfigResponse.StatusCode != HttpStatusCode.OK)
                    throw new InvalidDataException($"/sdkcom/v2/sys/player-config.json: expected HTTP 200, got {(int)playerConfigResponse.StatusCode} {playerConfigResponse.StatusCode}. Body: {playerConfigBody}");

                AssertKuroSdkSystemConfigData(JObject.Parse(playerConfigBody), "/sdkcom/v2/sys/player-config.json");
            }
            finally
            {
                await app.StopAsync();
                Environment.SetEnvironmentVariable("ASCNET_PUBLIC_HTTP_ORIGIN", previousPublicHttpOrigin);
            }
        }

        private static WebApplication CreateKuroSdkTestApp()
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = ["--urls", "http://127.0.0.1:0"]
            });
            builder.Logging.ClearProviders();
            builder.WebHost.UseUrls("http://127.0.0.1:0");

            WebApplication app = builder.Build();
            Type kuroSdkController = Type.GetType("AscNet.SDKServer.Controllers.KuroSdkController, AscNet.SDKServer", throwOnError: true)!;
            kuroSdkController.GetMethod("Register", BindingFlags.Public | BindingFlags.Static)!.Invoke(null, [app]);
            return app;
        }

        private static WebApplication CreateConfigControllerTestApp()
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = ["--urls", "http://127.0.0.1:0"]
            });
            builder.Logging.ClearProviders();
            builder.WebHost.UseUrls("http://127.0.0.1:0");

            WebApplication app = builder.Build();
            Type configController = Type.GetType("AscNet.SDKServer.Controllers.ConfigController, AscNet.SDKServer", throwOnError: true)!;
            configController.GetMethod("Register", BindingFlags.Public | BindingFlags.Static)!.Invoke(null, [app]);
            return app;
        }

        private static string BoundAddress(WebApplication app)
        {
            IServerAddressesFeature? addressesFeature = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>();
            string? address = addressesFeature?.Addresses.FirstOrDefault(address => address.StartsWith("http://127.0.0.1:", StringComparison.Ordinal));

            if (address is null)
                throw new InvalidDataException("Test server did not publish a loopback HTTP address.");

            return address;
        }

        private static void AssertKuroSdkSuccessEnvelope(JObject payload, string endpoint)
        {
            AssertKuroSdkSuccessCodeAndMessage(payload, endpoint);
            _ = RequiredObject(payload, "data", endpoint);
        }

        private static void AssertKuroSdkLoginData(JObject data, string endpoint)
        {
            _ = RequiredValue<int>(data, "id", JTokenType.Integer, endpoint);
            AssertEqual(KuroSdkDummyCuid, RequiredNonEmptyString(data, "cuid", endpoint), $"{endpoint} cuid");
            AssertEqual(KuroSdkDummyUsername, RequiredNonEmptyString(data, "username", endpoint), $"{endpoint} username");
            AssertEqual(KuroSdkDummyLoginType, RequiredValue<int>(data, "loginType", JTokenType.Integer, endpoint), $"{endpoint} loginType");
            _ = RequiredNonEmptyString(data, "code", endpoint);
            AssertEqual(KuroSdkDummyEmail, RequiredNonEmptyString(data, "email", endpoint), $"{endpoint} email");
            AssertEqual(KuroSdkDummyToken, RequiredNonEmptyString(data, "autoToken", endpoint), $"{endpoint} autoToken");
            AssertEqual(KuroSdkDummyToken, RequiredNonEmptyString(data, "token", endpoint), $"{endpoint} token");
            _ = RequiredValue<int>(data, "bindDevStat", JTokenType.Integer, endpoint);
            _ = RequiredValue<int>(data, "idStat", JTokenType.Integer, endpoint);
            _ = RequiredValue<int>(data, "firstLgn", JTokenType.Integer, endpoint);
            _ = RequiredValue<string>(data, "bindDevMsg", JTokenType.String, endpoint);
            _ = RequiredValue<int>(data, "realNameMethod", JTokenType.Integer, endpoint);
            _ = RequiredValue<string>(data, "thirdNickName", JTokenType.String, endpoint);
            _ = RequiredValue<int>(data, "bindDevSwitch", JTokenType.Integer, endpoint);
            _ = RequiredValue<string>(data, "realNameUrl", JTokenType.String, endpoint);
            _ = RequiredValue<string>(data, "realNameKey", JTokenType.String, endpoint);
        }

        private static void AssertKuroSdkAccessTokenData(JObject data, string endpoint)
        {
            _ = RequiredNonEmptyString(data, "access_token", endpoint);
            _ = RequiredValue<int>(data, "expires_in", JTokenType.Integer, endpoint);
        }

        private static void AssertKuroSdkOauthCodeData(JObject data, string endpoint)
        {
            _ = RequiredNonEmptyString(data, "oauthCode", endpoint);
            _ = RequiredNonEmptyString(data, "code", endpoint);
        }
        private static void AssertKuroSdkThirdLoginMarkData(JObject data, string endpoint)
        {
            AssertEqual(1, RequiredValue<int>(data, "ready", JTokenType.Integer, endpoint), $"{endpoint} ready");
            AssertEqual(KuroSdkDummyMark, RequiredNonEmptyString(data, "mark", endpoint), $"{endpoint} mark");
        }



        private static void AssertKuroSdkSystemConfigData(JObject data, string endpoint)
        {
            JArray links = (JArray)RequiredToken(data, "link", JTokenType.Array, endpoint);
            if (links.Count == 0)
                throw new InvalidDataException($"{endpoint} link: expected at least one CDN entry.");
            if (links[0] is not JObject firstLink)
                throw new InvalidDataException($"{endpoint} link[0]: expected JSON Object, got {links[0].Type}.");
            _ = RequiredNonEmptyString(firstLink, "url", $"{endpoint} link[0]");
            _ = RequiredValue<int>(firstLink, "weight", JTokenType.Integer, $"{endpoint} link[0]");
            _ = RequiredValue<int>(data, "clientSwitch", JTokenType.Integer, endpoint);
            _ = RequiredNonEmptyString(data, "pcGeetestUrl", endpoint);
            _ = RequiredNonEmptyString(data, "accCenterUrl", endpoint);
            JObject clientUrlConfig = JObject.Parse(RequiredNonEmptyString(data, "clientUrl", endpoint));
            _ = RequiredNonEmptyString(clientUrlConfig, "pcGeetestUrl", $"{endpoint} clientUrl");
            _ = RequiredNonEmptyString(clientUrlConfig, "accCenterUrl", $"{endpoint} clientUrl");
            AssertKuroSdkLocalLoginUrl(RequiredNonEmptyString(clientUrlConfig, "pcThirdLoginUrl", $"{endpoint} clientUrl"), $"{endpoint} clientUrl pcThirdLoginUrl");
            _ = RequiredNonEmptyString(clientUrlConfig, "kefu", $"{endpoint} clientUrl");
            _ = RequiredNonEmptyString(clientUrlConfig, "kefuServ", $"{endpoint} clientUrl");
            _ = RequiredValue<int>(clientUrlConfig, "sobot", JTokenType.Integer, $"{endpoint} clientUrl");
            _ = RequiredNonEmptyString(clientUrlConfig, "sobotRedDotUrl", $"{endpoint} clientUrl");
            _ = RequiredNonEmptyString(clientUrlConfig, "googlePcAuthResultUrl", $"{endpoint} clientUrl");
            _ = RequiredNonEmptyString(clientUrlConfig, "emailSystemUrl", $"{endpoint} clientUrl");
            _ = RequiredNonEmptyString(clientUrlConfig, "bizsiren", $"{endpoint} clientUrl");
            AssertKuroSdkLocalLoginUrl(RequiredNonEmptyString(data, "pcThirdLoginUrl", endpoint), $"{endpoint} pcThirdLoginUrl");
            _ = RequiredValue<int>(data, "thirdLogin", JTokenType.Integer, endpoint);
            _ = RequiredNonEmptyString(data, "kefu", endpoint);
            _ = RequiredNonEmptyString(data, "kefuServ", endpoint);
            _ = RequiredValue<int>(data, "sobot", JTokenType.Integer, endpoint);
            _ = RequiredNonEmptyString(data, "sobotRedDotUrl", endpoint);
            _ = RequiredNonEmptyString(data, "googlePcAuthResultUrl", endpoint);
            _ = RequiredNonEmptyString(data, "emailSystemUrl", endpoint);
            _ = RequiredValue<int>(data, "webviewIgnoreCertErrorForWin", JTokenType.Integer, endpoint);
            _ = RequiredValue<int>(data, "disableAgreementRemainDlgForWin", JTokenType.Integer, endpoint);
            _ = RequiredValue<int>(data, "PayEventFixedForPCSteam", JTokenType.Integer, endpoint);
        }

        private static void AssertKuroSdkSuccessCodeAndMessage(JObject payload, string endpoint)
        {
            AssertEqual(0, RequiredValue<int>(payload, "code", JTokenType.Integer, endpoint), $"{endpoint} code");
            AssertEqual("success", RequiredValue<string>(payload, "msg", JTokenType.String, endpoint), $"{endpoint} msg");
        }

        private static void AssertKuroSdkLocalLoginUrl(string value, string name, string? expectedMark = null, string? expectedType = null)
        {
            if (!Uri.TryCreate(value, UriKind.Absolute, out Uri? uri))
                throw new InvalidDataException($"{name}: expected an absolute local SDK URL, got '{value}'.");

            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                throw new InvalidDataException($"{name}: expected HTTP(S) local SDK URL, got '{value}'.");

            if (!uri.IsLoopback)
                throw new InvalidDataException($"{name}: expected loopback local SDK URL, got '{value}'.");

            AssertEqual("/sdkcom/v2/local/login", uri.AbsolutePath, name);

            if (expectedMark is not null || expectedType is not null)
            {
                Dictionary<string, string> query = ParseQueryString(uri.Query);
                if (expectedMark is not null)
                    AssertEqual(expectedMark, query.GetValueOrDefault("mark"), $"{name} mark");
                if (expectedType is not null)
                    AssertEqual(expectedType, query.GetValueOrDefault("type"), $"{name} type");
            }
        }


        private static void AssertKuroSdkEmptyData(JObject data, string endpoint)
        {
        }

        private static void AssertKuroSdkRealNameCheckData(JObject data, string endpoint)
        {
            _ = RequiredValue<int>(data, "realNameMethod", JTokenType.Integer, endpoint);
            _ = RequiredValue<string>(data, "realNameKey", JTokenType.String, endpoint);
            _ = RequiredValue<string>(data, "realNameUrl", JTokenType.String, endpoint);
        }

        private static JObject RequiredObject(JObject payload, string propertyName, string endpoint)
        {
            return (JObject)RequiredToken(payload, propertyName, JTokenType.Object, endpoint);
        }

        private static string RequiredNonEmptyString(JObject payload, string propertyName, string endpoint)
        {
            string value = RequiredValue<string>(payload, propertyName, JTokenType.String, endpoint);
            if (value.Length == 0)
                throw new InvalidDataException($"{endpoint} {propertyName}: expected non-empty string.");

            return value;
        }

        private static T RequiredValue<T>(JObject payload, string propertyName, JTokenType expectedType, string endpoint)
        {
            return RequiredToken(payload, propertyName, expectedType, endpoint).Value<T>()!;
        }

        private static JToken RequiredToken(JObject payload, string propertyName, JTokenType expectedType, string endpoint)
        {
            if (!payload.TryGetValue(propertyName, out JToken? token))
                throw new InvalidDataException($"{endpoint}: missing JSON field '{propertyName}'.");

            if (token.Type != expectedType)
                throw new InvalidDataException($"{endpoint} {propertyName}: expected JSON {expectedType}, got {token.Type}.");

            return token;
        }

        private static Dictionary<string, string> ParseQueryString(string query)
        {
            Dictionary<string, string> result = new();
            foreach (string part in query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
            {
                int separator = part.IndexOf('=');
                string key = separator >= 0 ? part[..separator] : part;
                string value = separator >= 0 ? part[(separator + 1)..] : string.Empty;
                result[Uri.UnescapeDataString(key)] = Uri.UnescapeDataString(value);
            }

            return result;
        }

        private static Dictionary<string, string> KuroSdkHandoffFields(string? url = null, string? mark = null)
        {
            Dictionary<string, string> fields = new()
            {
                ["email"] = KuroSdkDummyEmail,
                ["token"] = KuroSdkDummyToken,
                ["steamId"] = KuroSdkDummySteamId,
                ["cuid"] = KuroSdkDummyCuid,
                ["username"] = KuroSdkDummyUsername,
                ["loginType"] = KuroSdkDummyLoginType.ToString()
            };

            if (url is not null)
                fields["url"] = url;
            if (mark is not null)
                fields["mark"] = mark;

            return fields;
        }

        private static string KuroSdkHandoffQuery(string? url = null, string? mark = null)
        {
            return string.Join("&", KuroSdkHandoffFields(url, mark).Select(field => $"{Uri.EscapeDataString(field.Key)}={Uri.EscapeDataString(field.Value)}"));
        }

        private sealed record KuroSdkEndpointContract(string Path, Action<JObject, string> AssertData);

        private static string ConfigValue(List<RemoteConfig> remoteConfigs, string key)
        {
            return remoteConfigs.Single(config => config.Key == key).Value;
        }

        private static void UseResourceWorkingDirectory()
        {
            if (!File.Exists("Configs/version_config.json") && Directory.Exists("Resources/Configs"))
                Directory.SetCurrentDirectory("Resources");
        }

        private static string ResourcePath(params string[] segments)
        {
            string[][] candidates =
            [
                segments,
                ["Resources", ..segments],
                ["..", "Resources", ..segments],
            ];

            foreach (string[] candidate in candidates)
            {
                string path = Path.Combine(candidate);
                if (File.Exists(path))
                    return path;
            }

            throw new FileNotFoundException($"Resource file not found: {Path.Combine(segments)}");
        }

        private static void AssertEqual<T>(T expected, T actual, string name)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
                throw new InvalidDataException($"{name}: expected '{expected}', got '{actual}'.");
        }

        private static void AssertEmptyList<T>(ICollection<T>? values, string name)
        {
            if (values is null)
                throw new InvalidDataException($"{name}: expected an empty list, got nil.");
            if (values.Count != 0)
                throw new InvalidDataException($"{name}: expected an empty list, got {values.Count} entries.");
        }

        class PropertyCompareResult
        {
            public string Name { get; private set; }
            public object OldValue { get; private set; }
            public object NewValue { get; private set; }

            public PropertyCompareResult(string name, object oldValue, object newValue)
            {
                Name = name;
                OldValue = oldValue;
                NewValue = newValue;
            }
        }

        class IgnorePropertyCompareAttribute : Attribute { }

        private static List<PropertyCompareResult> Compare<T>(T oldObject, T newObject, Type typecast = null)
        {
            PropertyInfo[] properties = null;
            if (typecast != null)
            {
                properties = typecast.GetProperties();
            }
            else
            {
                properties = typeof(T).GetProperties();
            }
            List<PropertyCompareResult> result = new List<PropertyCompareResult>();

            foreach (PropertyInfo pi in properties)
            {
                if (pi.CustomAttributes.Any(ca => ca.AttributeType == typeof(IgnorePropertyCompareAttribute)))
                {
                    continue;
                }

                object oldValue = pi.GetValue(oldObject), newValue = pi.GetValue(newObject);
                if (oldValue is null || newValue is null)
                {
                    if (!object.Equals(oldValue, newValue))
                        result.Add(new PropertyCompareResult(pi.Name, oldValue, newValue));
                    continue;
                }

                if (!object.Equals(oldValue, newValue))
                {
                    PropertyInfo[] propertyInfos = oldValue.GetType().GetProperties();
                    if (propertyInfos.Length > 1 && oldValue.GetType().IsClass && !oldValue.GetType().IsArray && !oldValue.GetType().IsGenericType)
                    {
                        result.AddRange(Compare(oldValue, newValue, oldValue.GetType()));
                    }
                    else
                    {
                        result.Add(new PropertyCompareResult(pi.Name, oldValue, newValue));
                    }
                }
            }

            return result;
        }

    }
}