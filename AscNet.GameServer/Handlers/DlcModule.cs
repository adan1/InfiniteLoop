using MessagePack;

namespace AscNet.GameServer.Handlers
{

    #region MsgPackScheme
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [MessagePackObject(true)]
    public class DlcQuestUpdateResponse
    {
        public int Code;
    }

    [MessagePackObject(true)]
    public class DlcWorldSaveDataResponse
    {
        public int Code;
        public object? WorldSaveData;
    }

    [MessagePackObject(true)]
    public class DlcWorldSceneObjectDataResponse
    {
        public int Code;
        public Dictionary<int, object> SceneObjectStates = new();
    }

    [MessagePackObject(true)]
    public class DlcSceneObjectStateSetResponse
    {
        public int Code;
        public List<object> RewardGoods = new();
    }

    [MessagePackObject(true)]
    public class BigWorldCurNpcPosUpdateResponse
    {
        public int Code;
    }

    [MessagePackObject(true)]
    public class BigWorldCafeNewRoundResponse
    {
        public int Code;
        public object? CafeGambling;
    }

    [MessagePackObject(true)]
    public class BigWorldCafeNextRoundResponse
    {
        public int Code;
    }

    [MessagePackObject(true)]
    public class BigWorldCafeCardGroupListSaveResponse
    {
        public int Code;
    }

    [MessagePackObject(true)]
    public class BigWorldCafeGiveUpResponse
    {
        public int Code;
    }

    [MessagePackObject(true)]
    public class BigWorldCafeGuideKickOutSceneResponse
    {
        public int Code;
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    #endregion

    internal class DlcModule
    {
        [RequestPacketHandler("DlcQuestUpdateRequest")]
        public static void DlcQuestUpdateRequestHandler(Session session, Packet.Request packet)
        {
            session.SendResponse(new DlcQuestUpdateResponse(), packet.Id);
        }

        [RequestPacketHandler("DlcWorldSaveDataRequest")]
        public static void DlcWorldSaveDataRequestHandler(Session session, Packet.Request packet)
        {
            session.SendResponse(new DlcWorldSaveDataResponse(), packet.Id);
        }

        [RequestPacketHandler("DlcWorldSceneObjectDataRequest")]
        public static void DlcWorldSceneObjectDataRequestHandler(Session session, Packet.Request packet)
        {
            session.SendResponse(new DlcWorldSceneObjectDataResponse(), packet.Id);
        }

        [RequestPacketHandler("DlcSceneObjectStateSetRequest")]
        public static void DlcSceneObjectStateSetRequestHandler(Session session, Packet.Request packet)
        {
            session.SendResponse(new DlcSceneObjectStateSetResponse(), packet.Id);
        }

        [RequestPacketHandler("DlcWorldEnterSucceedRequest")]
        public static void DlcWorldEnterSucceedRequestHandler(Session session, Packet.Request packet)
        {
        }

        [RequestPacketHandler("DlcSingleFightSettleRequest")]
        public static void DlcSingleFightSettleRequestHandler(Session session, Packet.Request packet)
        {
        }

        [RequestPacketHandler("BigWorldCurNpcPosUpdateRequest")]
        public static void BigWorldCurNpcPosUpdateRequestHandler(Session session, Packet.Request packet)
        {
            session.SendResponse(new BigWorldCurNpcPosUpdateResponse(), packet.Id);
        }

        [RequestPacketHandler("BigWorldCafeNewRoundRequest")]
        public static void BigWorldCafeNewRoundRequestHandler(Session session, Packet.Request packet)
        {
            session.SendResponse(new BigWorldCafeNewRoundResponse(), packet.Id);
        }

        [RequestPacketHandler("BigWorldCafeNextRoundRequest")]
        public static void BigWorldCafeNextRoundRequestHandler(Session session, Packet.Request packet)
        {
            session.SendResponse(new BigWorldCafeNextRoundResponse(), packet.Id);
        }

        [RequestPacketHandler("BigWorldCafeCardGroupListSaveRequest")]
        public static void BigWorldCafeCardGroupListSaveRequestHandler(Session session, Packet.Request packet)
        {
            session.SendResponse(new BigWorldCafeCardGroupListSaveResponse(), packet.Id);
        }

        [RequestPacketHandler("BigWorldCafeGiveUpRequest")]
        public static void BigWorldCafeGiveUpRequestHandler(Session session, Packet.Request packet)
        {
            session.SendResponse(new BigWorldCafeGiveUpResponse(), packet.Id);
        }

        [RequestPacketHandler("BigWorldCafeGuideKickOutSceneRequest")]
        public static void BigWorldCafeGuideKickOutSceneRequestHandler(Session session, Packet.Request packet)
        {
            session.SendResponse(new BigWorldCafeGuideKickOutSceneResponse(), packet.Id);
        }
    }
}
