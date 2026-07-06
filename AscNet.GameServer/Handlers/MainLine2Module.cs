using AscNet.Common.MsgPack;
using MessagePack;

namespace AscNet.GameServer.Handlers
{
    #region MsgPackScheme
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [MessagePackObject(true)]
    public class MainLine2UpdateExhibitionChapterRequest
    {
        public int ChapterId { get; set; }
    }

    [MessagePackObject(true)]
    public class MainLine2UpdateExhibitionChapterResponse
    {
        public int Code { get; set; }
    }

    [MessagePackObject(true)]
    public class MainLineLuosaitaEnterRequest
    {
        public int SectionId { get; set; }
    }

    [MessagePackObject(true)]
    public class MainLineLuosaitaEnterResponse
    {
        public int Code { get; set; }
        public MainLineLuosaitaSectionInfo SectionInfo { get; set; }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    #endregion

    internal class MainLine2Module
    {
        [RequestPacketHandler("MainLine2UpdateExhibitionChapterRequest")]
        public static void MainLine2UpdateExhibitionChapterRequestHandler(Session session, Packet.Request packet)
        {
            _ = MessagePackSerializer.Deserialize<MainLine2UpdateExhibitionChapterRequest>(packet.Content);

            session.SendResponse(new MainLine2UpdateExhibitionChapterResponse
            {
                Code = 0
            }, packet.Id);
        }

        [RequestPacketHandler("MainLineLuosaitaEnterRequest")]
        public static void MainLineLuosaitaEnterRequestHandler(Session session, Packet.Request packet)
        {
            MainLineLuosaitaEnterRequest request = MessagePackSerializer.Deserialize<MainLineLuosaitaEnterRequest>(packet.Content);

            session.SendResponse(new MainLineLuosaitaEnterResponse
            {
                Code = 0,
                SectionInfo = MainLineLuosaitaPayloadFactory.BuildCapturedSectionInfo()
            }, packet.Id);
        }
    }

    public static class MainLineLuosaitaPayloadFactory
    {
        public static FubenMainLineLuosaitaData BuildLoginData()
        {
            return new FubenMainLineLuosaitaData
            {
                IncId = 29,
                SectionInfos = [BuildCapturedSectionInfo()],
                KillEnemySet = [201, 202, 203]
            };
        }

        public static MainLineLuosaitaSectionInfo BuildCapturedSectionInfo()
        {
            return new MainLineLuosaitaSectionInfo
            {
                SectionId = 1,
                BlockInfos =
                [
                    new() { Id = 101, BlockStatus = 1 },
                    new() { Id = 102, BlockStatus = 1 },
                    new() { Id = 103, BlockStatus = 1 },
                    new() { Id = 104, BlockStatus = 1 },
                    new() { Id = 105, BlockStatus = 0 },
                    new() { Id = 106, BlockStatus = 0 },
                    new() { Id = 107, BlockStatus = 0 },
                    new() { Id = 108, BlockStatus = 0 },
                    new() { Id = 109, BlockStatus = 0 },
                    new() { Id = 110, BlockStatus = 0 }
                ],
                SectionMembers =
                [
                    new()
                    {
                        Guid = 1,
                        Type = 1,
                        BlockId = 104,
                        PosId = 4,
                        ArmyInfo = new() { Id = 101, CurHp = 8, ExtraAttack = 0 }
                    },
                    new()
                    {
                        Guid = 9,
                        Type = 4,
                        BlockId = 104,
                        PosId = 9,
                        StageId = 10380101
                    },
                    new()
                    {
                        Guid = 13,
                        Type = 2,
                        BlockId = 105,
                        PosId = 13,
                        EnemyInfo = new() { Id = 204, CurHp = 9, ExtraAttack = 0 }
                    },
                    new()
                    {
                        Guid = 14,
                        Type = 2,
                        BlockId = 106,
                        PosId = 14,
                        EnemyInfo = new() { Id = 205, CurHp = 3, ExtraAttack = 0 }
                    },
                    new()
                    {
                        Guid = 15,
                        Type = 2,
                        BlockId = 107,
                        PosId = 15,
                        EnemyInfo = new() { Id = 206, CurHp = 7, ExtraAttack = 0 }
                    },
                    new()
                    {
                        Guid = 16,
                        Type = 2,
                        BlockId = 108,
                        PosId = 16,
                        EnemyInfo = new() { Id = 207, CurHp = 16, ExtraAttack = 0 }
                    },
                    new()
                    {
                        Guid = 17,
                        Type = 2,
                        BlockId = 109,
                        PosId = 17,
                        EnemyInfo = new() { Id = 208, CurHp = 17, ExtraAttack = 0 }
                    },
                    new()
                    {
                        Guid = 26,
                        Type = 2,
                        BlockId = 107,
                        PosId = 26,
                        EnemyInfo = new() { Id = 239, CurHp = 99, ExtraAttack = 0 }
                    },
                    new()
                    {
                        Guid = 27,
                        Type = 2,
                        BlockId = 110,
                        PosId = 27,
                        EnemyInfo = new() { Id = 240, CurHp = 99, ExtraAttack = 0 }
                    },
                    new()
                    {
                        Guid = 28,
                        Type = 3,
                        BlockId = 104,
                        PosId = 5,
                        CharacterId = 3107
                    },
                    new()
                    {
                        Guid = 29,
                        Type = 3,
                        BlockId = 104,
                        PosId = 6,
                        CharacterId = 3101
                    }
                ],
                DocList = [],
                CharacterMoveIds = [],
                SectionStatus = 0
            };
        }
    }
}
