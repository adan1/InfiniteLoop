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
    }
}
