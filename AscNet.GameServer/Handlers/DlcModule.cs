using AscNet.Common.Util;
using System.Buffers;
using MessagePack;
using Newtonsoft.Json.Linq;

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
    public class DlcWorldSaveDataRequest
    {
        public int WorldId;
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
    public class BigWorldEnterWorldRequest
    {
        public int WorldId;
        public int LevelId;
    }

    [MessagePackObject(true)]
    public class BigWorldEnterWorldResponse
    {
        public int Code;
        public object? EnterResultData;
        public object? PlayerData;
        public object? DlcQuestBag;
    }

    [MessagePackObject(true)]
    public class BigWorldOnModuleLoadCompleteRequest
    {
    }

    [MessagePackObject(true)]
    public class BigWorldOnModuleLoadCompleteResponse
    {
        public int Code;
    }

    [MessagePackObject(true)]
    public class BigWorldSaveFovDataRequest
    {
        public int FovGroupId;
        public int FovType;
    }

    [MessagePackObject(true)]
    public class BigWorldSaveFovDataResponse
    {
        public int Code;
    }

    [MessagePackObject(true)]
    public class BigWorldTeamChangeRequest
    {
        public object? ChangeTeam;
    }

    [MessagePackObject(true)]
    public class BigWorldTeamChangeResponse
    {
        public int Code;
    }

    [MessagePackObject(true)]
    public class BigWorldCheckIsShowMainRedPointRequest
    {
        public int SysModuleId;
    }

    [MessagePackObject(true)]
    public class BigWorldCheckIsShowMainRedPointResponse
    {
        public int Code;
    }

    [MessagePackObject(true)]
    public class NotifyBigWorldAlbumUpdate
    {
        public object? AlbumData;
    }

    [MessagePackObject(true)]
    public class NotifyBigWorldMapData
    {
        public Dictionary<int, int> BoxRewardedCntData = new();
    }

    [MessagePackObject(true)]
    public class NotifyBigWorldMainRedPoint
    {
        public List<object> RedPoints = new();
    }

    [MessagePackObject(true)]
    public class StartFightNotify
    {
        public int Code;
    }

    [MessagePackObject(true)]
    public class XRpcCommonResponse
    {
        public int Code;
    }

    [MessagePackObject(true)]
    public class XRpcComponentActionResponse
    {
        public int Code;
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
        private const string BigWorldEnterWorldPayloadPath = "Configs/big_world_enter_world_response.msgpack.b64";
        private const string BigWorldSaveDataPayloadPath = "Configs/big_world_save_data_response.msgpack.b64";
        private const string BigWorldAlbumUpdatePayloadPath = "Configs/big_world_album_update.msgpack.b64";
        private const string BigWorldMapDataPayloadPath = "Configs/big_world_map_data.msgpack.b64";
        private const string BigWorldSgDormDataPayloadPath = "Configs/big_world_sg_dorm_data.msgpack.b64";
        private const string BigWorldLoadCompleteXRpcPushesSnapshotPath = "Configs/big_world_load_complete_xrpc_pushes.json";
        // XRpcCommon push envelope tail verified against retail interaction/load-complete captures.
        private const byte BigWorldXRpcServerControllerId = 1;
        private const byte BigWorldXRpcCommonOpcode = 15;
        private const byte BigWorldXRpcNoLevelId = 0;
        private static readonly Lazy<byte[]> BigWorldEnterWorldPayload = new(() => LoadBase64Payload(BigWorldEnterWorldPayloadPath));
        private static readonly Lazy<byte[]> BigWorldSaveDataPayload = new(() => LoadBase64Payload(BigWorldSaveDataPayloadPath));
        private static readonly Lazy<byte[]> BigWorldAlbumUpdatePayload = new(() => LoadBase64Payload(BigWorldAlbumUpdatePayloadPath));
        private static readonly Lazy<byte[]> BigWorldMapDataPayload = new(() => LoadBase64Payload(BigWorldMapDataPayloadPath));
        private static readonly Lazy<byte[]> BigWorldSgDormDataPayload = new(() => LoadBase64Payload(BigWorldSgDormDataPayloadPath));
        private static readonly Lazy<JObject> BigWorldEnterWorldFixture = new(() => JObject.Parse(MessagePackSerializer.ConvertToJson(BigWorldEnterWorldPayload.Value)));
        private static readonly Lazy<(long PlayerId, string PlayerName)> BigWorldFixturePlayer = new(() => ReadBigWorldFixturePlayer(BigWorldEnterWorldFixture.Value));
        private static readonly Lazy<(int WorldId, int LevelId)> BigWorldFixtureWorld = new(() => ReadBigWorldFixtureWorld(BigWorldEnterWorldFixture.Value));
        private static readonly Lazy<IReadOnlyList<(int PartId, int ColourId)>> BigWorldFixtureCommanderParts = new(() => ReadBigWorldFixtureCommanderParts(BigWorldEnterWorldFixture.Value));
        private static readonly Lazy<(IReadOnlyList<int> EnteredBigWorldIds, int Gender, IReadOnlyList<int> CommanderFashionBags)> BigWorldExternalRequiredPlayerData = new(() => ReadBigWorldExternalRequiredPlayerData(BigWorldEnterWorldFixture.Value));
        private static readonly Lazy<JObject> BigWorldLoadCompleteXRpcPushesSnapshot = new(() => JsonSnapshot.LoadObject(BigWorldLoadCompleteXRpcPushesSnapshotPath));

        [RequestPacketHandler("BigWorldEnterWorldRequest")]
        public static void BigWorldEnterWorldRequestHandler(Session session, Packet.Request packet)
        {
            _ = MessagePackSerializer.Deserialize<BigWorldEnterWorldRequest>(packet.Content);
            session.SendResponse(nameof(BigWorldEnterWorldResponse), BuildBigWorldEnterWorldPayload(session), packet.Id);
            session.SendPush(nameof(NotifyBigWorldAlbumUpdate), BigWorldAlbumUpdatePayload.Value);
            session.SendPush(nameof(NotifyBigWorldMapData), BigWorldMapDataPayload.Value);
            session.SendPush("NotifySgDormData", BigWorldSgDormDataPayload.Value);
        }

        [RequestPacketHandler("BigWorldOnModuleLoadCompleteRequest")]
        public static void BigWorldOnModuleLoadCompleteRequestHandler(Session session, Packet.Request packet)
        {
            session.SendResponse(new BigWorldOnModuleLoadCompleteResponse(), packet.Id);
        }

        [RequestPacketHandler("BigWorldSaveFovDataRequest")]
        public static void BigWorldSaveFovDataRequestHandler(Session session, Packet.Request packet)
        {
            session.SendResponse(new BigWorldSaveFovDataResponse(), packet.Id);
        }

        [RequestPacketHandler("BigWorldTeamChangeRequest")]
        public static void BigWorldTeamChangeRequestHandler(Session session, Packet.Request packet)
        {
            session.SendResponse(new BigWorldTeamChangeResponse(), packet.Id);
        }

        [RequestPacketHandler("BigWorldCheckIsShowMainRedPointRequest")]
        public static void BigWorldCheckIsShowMainRedPointRequestHandler(Session session, Packet.Request packet)
        {
            session.SendPush(new NotifyBigWorldMainRedPoint());
            session.SendResponse(new BigWorldCheckIsShowMainRedPointResponse(), packet.Id);
        }

        [RequestPacketHandler("DlcQuestUpdateRequest")]
        public static void DlcQuestUpdateRequestHandler(Session session, Packet.Request packet)
        {
            session.SendResponse(new DlcQuestUpdateResponse(), packet.Id);
        }

        [RequestPacketHandler("DlcWorldSaveDataRequest")]
        public static void DlcWorldSaveDataRequestHandler(Session session, Packet.Request packet)
        {
            _ = MessagePackSerializer.Deserialize<DlcWorldSaveDataRequest>(packet.Content);
            session.SendResponse(nameof(DlcWorldSaveDataResponse), BigWorldSaveDataPayload.Value, packet.Id);
            session.PendingBigWorldLoadCompleteXRpc = true;
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

        [RequestPacketHandler("XRpcCommon")]
        public static void XRpcCommonRequestHandler(Session session, Packet.Request packet)
        {
            TrySendBigWorldInteractNotifies(session, packet.Content);
            session.SendResponse(new XRpcCommonResponse(), packet.Id);
        }

        private static void TrySendBigWorldInteractNotifies(Session session, byte[] payload)
        {
            try
            {
                if (!TryReadBigWorldPlayerInteractRequest(payload, out int targetUuid, out int targetPlaceId, out int targetType, out int optionId))
                    return;

                session.SendPush("XRpcCommon", BuildBigWorldXRpcPayload(
                    "RpcNpcInteractStartNotify",
                    SerializeBigWorldXRpcIntegerArgs(targetUuid, targetPlaceId, targetType, optionId),
                    levelId: BigWorldXRpcNoLevelId));
                session.SendPush("XRpcCommon", BuildBigWorldXRpcPayload(
                    "RpcNpcInteractFinishNotify",
                    SerializeBigWorldXRpcIntegerArgs(),
                    levelId: BigWorldXRpcNoLevelId));
            }
            catch
            {
                // Interaction completion pushes are best-effort; the normal XRpc ack must still be delivered.
            }
        }

        private static bool TryReadBigWorldPlayerInteractRequest(byte[] payload, out int targetUuid, out int targetPlaceId, out int targetType, out int optionId)
        {
            targetUuid = 0;
            targetPlaceId = 0;
            targetType = 0;
            optionId = 0;

            object?[] rpc = MessagePackSerializer.Deserialize<object?[]>(payload);
            if (rpc.Length < 2 || rpc[0] is not string rpcName || rpcName != "RpcPlayerInteractRequest" || rpc[1] is not byte[] argsPayload)
                return false;

            object?[] args = MessagePackSerializer.Deserialize<object?[]>(argsPayload);
            if (args.Length < 8)
                return false;

            targetUuid = ReadBigWorldXRpcInt32(args[3]);
            targetPlaceId = ReadBigWorldXRpcInt32(args[4]);
            targetType = ReadBigWorldXRpcInt32(args[5]);
            optionId = ReadBigWorldXRpcInt32(args[7]);
            return true;
        }

        private static int ReadBigWorldXRpcInt32(object? value)
        {
            return value switch
            {
                byte typed => typed,
                sbyte typed => typed,
                short typed => typed,
                ushort typed => typed,
                int typed => typed,
                uint typed when typed <= int.MaxValue => (int)typed,
                long typed when typed >= int.MinValue && typed <= int.MaxValue => (int)typed,
                ulong typed when typed <= int.MaxValue => (int)typed,
                _ => throw new MessagePackSerializationException($"Expected BigWorld XRpc int32-compatible value, got {value?.GetType().FullName ?? "null"}.")
            };
        }

        private static byte[] BuildBigWorldXRpcPayload(string rpcName, byte[] argsPayload, byte levelId)
        {
            ArrayBufferWriter<byte> buffer = new();
            MessagePackWriter writer = new(buffer);
            writer.WriteArrayHeader(5);
            writer.Write(rpcName);
            writer.Write(argsPayload);
            writer.Write(BigWorldXRpcServerControllerId);
            writer.Write(BigWorldXRpcCommonOpcode);
            writer.Write(levelId);
            writer.Flush();
            return buffer.WrittenMemory.ToArray();
        }

        private static byte[] SerializeBigWorldXRpcIntegerArgs(params int[] args)
        {
            ArrayBufferWriter<byte> buffer = new();
            MessagePackWriter writer = new(buffer);
            writer.WriteArrayHeader(args.Length);
            foreach (int arg in args)
                WriteBigWorldXRpcInteger(ref writer, arg);

            writer.Flush();
            return buffer.WrittenMemory.ToArray();
        }

        private static void WriteBigWorldXRpcInteger(ref MessagePackWriter writer, int value)
        {
            if (value < 0)
            {
                writer.Write(value);
                return;
            }

            if (value <= byte.MaxValue)
            {
                writer.Write((byte)value);
                return;
            }

            if (value <= ushort.MaxValue)
            {
                writer.Write((ushort)value);
                return;
            }

            writer.Write((uint)value);
        }

        [RequestPacketHandler("XRpcComponentAction")]
        public static void XRpcComponentActionRequestHandler(Session session, Packet.Request packet)
        {
            session.SendResponse(new XRpcComponentActionResponse(), packet.Id);
        }

        private static byte[] LoadBase64Payload(string relativePath)
        {
            string payload = JsonSnapshot.LoadText(relativePath).Trim();
            if (string.IsNullOrWhiteSpace(payload))
                throw new FileNotFoundException($"Required BigWorld fixture is missing or empty: {relativePath}");

            return Convert.FromBase64String(payload);
        }

        private static (long PlayerId, string PlayerName) ReadBigWorldFixturePlayer(JObject response)
        {
            JArray players = RequiredBigWorldFixtureArray(response["EnterResultData"]?["WorldData"]?["Players"], "EnterResultData.WorldData.Players");
            JObject firstPlayer = players.First as JObject
                ?? throw new InvalidDataException("BigWorld enter-world fixture is missing EnterResultData.WorldData.Players[0].");
            long playerId = RequiredBigWorldFixtureLong(firstPlayer, "Id", "EnterResultData.WorldData.Players[0]");
            string playerName = RequiredBigWorldFixtureString(firstPlayer, "Name", "EnterResultData.WorldData.Players[0]");
            return (playerId, playerName);
        }

        private static (int WorldId, int LevelId) ReadBigWorldFixtureWorld(JObject response)
        {
            JObject worldData = RequiredBigWorldFixtureObject(response["EnterResultData"]?["WorldData"], "EnterResultData.WorldData");
            return (
                RequiredBigWorldFixtureInt(worldData, "WorldId", "EnterResultData.WorldData"),
                RequiredBigWorldFixtureInt(worldData, "LevelId", "EnterResultData.WorldData"));
        }

        private static IReadOnlyList<(int PartId, int ColourId)> ReadBigWorldFixtureCommanderParts(JObject response)
        {
            JObject playerData = RequiredBigWorldFixtureObject(response["PlayerData"], "PlayerData");
            int outfitType = RequiredBigWorldFixtureInt(playerData, "CurCommanderOutfitType", "PlayerData");
            JObject outfits = RequiredBigWorldFixtureObject(playerData["CommanderFashionOutfits"], "PlayerData.CommanderFashionOutfits");
            JObject outfit = RequiredBigWorldFixtureObject(outfits[outfitType.ToString()], $"PlayerData.CommanderFashionOutfits[{outfitType}]");
            JObject wearFashionDict = RequiredBigWorldFixtureObject(outfit["WearFashionDict"], $"PlayerData.CommanderFashionOutfits[{outfitType}].WearFashionDict");

            List<(int Order, int PartId, int ColourId)> parts = new(wearFashionDict.Count);
            foreach (JProperty property in wearFashionDict.Properties())
            {
                int order = int.TryParse(property.Name, out int parsedOrder) ? parsedOrder : int.MaxValue;
                JObject part = RequiredBigWorldFixtureObject(property.Value, $"PlayerData.CommanderFashionOutfits[{outfitType}].WearFashionDict[{property.Name}]");
                parts.Add((
                    order,
                    RequiredBigWorldFixtureInt(part, "PartId", $"PlayerData.CommanderFashionOutfits[{outfitType}].WearFashionDict[{property.Name}]"),
                    RequiredBigWorldFixtureInt(part, "ColourId", $"PlayerData.CommanderFashionOutfits[{outfitType}].WearFashionDict[{property.Name}]")));
            }

            if (parts.Count == 0)
                throw new InvalidDataException($"BigWorld enter-world fixture is missing PlayerData.CommanderFashionOutfits[{outfitType}].WearFashionDict parts.");

            return parts
                .OrderBy(part => part.Order)
                .Select(part => (part.PartId, part.ColourId))
                .ToArray();
        }

        private static (IReadOnlyList<int> EnteredBigWorldIds, int Gender, IReadOnlyList<int> CommanderFashionBags) ReadBigWorldExternalRequiredPlayerData(JObject response)
        {
            JObject playerData = RequiredBigWorldFixtureObject(response["PlayerData"], "PlayerData");
            JArray commanderFashionBags = RequiredBigWorldFixtureArray(playerData["CommanderFashionBags"], "PlayerData.CommanderFashionBags");
            return (
                Array.Empty<int>(),
                RequiredBigWorldFixtureInt(playerData, "Gender", "PlayerData"),
                ReadBigWorldFixtureIntArray(commanderFashionBags, "PlayerData.CommanderFashionBags"));
        }

        internal static NotifyExternalRequiredBigWorldPlayerData BuildExternalRequiredBigWorldPlayerData()
        {
            (IReadOnlyList<int> enteredBigWorldIds, int gender, IReadOnlyList<int> commanderFashionBags) = BigWorldExternalRequiredPlayerData.Value;
            return new NotifyExternalRequiredBigWorldPlayerData
            {
                EnteredBigWorldIds = enteredBigWorldIds.ToList(),
                Gender = gender,
                CommanderFashionBags = commanderFashionBags.ToList()
            };
        }

        private static JObject RequiredBigWorldFixtureObject(JToken? token, string path)
        {
            return token as JObject
                ?? throw new InvalidDataException($"BigWorld enter-world fixture is missing object {path}.");
        }

        private static JArray RequiredBigWorldFixtureArray(JToken? token, string path)
        {
            return token as JArray
                ?? throw new InvalidDataException($"BigWorld enter-world fixture is missing array {path}.");
        }

        private static int RequiredBigWorldFixtureInt(JObject source, string name, string path)
        {
            return source.Value<int?>(name)
                ?? throw new InvalidDataException($"BigWorld enter-world fixture is missing integer {path}.{name}.");
        }

        private static long RequiredBigWorldFixtureLong(JObject source, string name, string path)
        {
            return source.Value<long?>(name)
                ?? throw new InvalidDataException($"BigWorld enter-world fixture is missing integer {path}.{name}.");
        }

        private static string RequiredBigWorldFixtureString(JObject source, string name, string path)
        {
            return source.Value<string>(name)
                ?? throw new InvalidDataException($"BigWorld enter-world fixture is missing string {path}.{name}.");
        }

        private static IReadOnlyList<int> ReadBigWorldFixtureIntArray(JArray array, string path)
        {
            int[] values = new int[array.Count];
            for (int index = 0; index < array.Count; index++)
            {
                values[index] = array[index]?.Value<int?>()
                    ?? throw new InvalidDataException($"BigWorld enter-world fixture is missing integer {path}[{index}].");
            }

            return values;
        }

        private static byte[] BuildBigWorldEnterWorldPayload(Session session)
        {
            byte[] payload = BigWorldEnterWorldPayload.Value;
            try
            {
                return PatchBigWorldEnterWorldPayload(session, payload);
            }
            catch (Exception ex)
            {
                session.log.Warn($"Falling back to unpatched BigWorldEnterWorldResponse payload: {ex.Message}");
                return payload;
            }
        }

        private static byte[] PatchBigWorldEnterWorldPayload(Session session, byte[] payload)
        {
            MessagePackReader reader = new(new ReadOnlySequence<byte>(payload));
            ArrayBufferWriter<byte> buffer = new(payload.Length + 64);
            MessagePackWriter writer = new(buffer);
            List<string> path = new(8);
            RewriteBigWorldEnterWorldValue(session, payload, ref reader, ref writer, path, false);
            if (reader.Consumed != payload.Length)
                throw new MessagePackSerializationException("BigWorld enter-world fixture contains trailing MessagePack data.");
            writer.Flush();
            return buffer.WrittenMemory.ToArray();
        }

        private static byte[] PatchBigWorldNestedNativePayload(Session session, byte[] payload)
        {
            MessagePackReader reader = new(new ReadOnlySequence<byte>(payload));
            ArrayBufferWriter<byte> buffer = new(payload.Length + 64);
            MessagePackWriter writer = new(buffer);
            List<string> path = new(8);
            RewriteBigWorldEnterWorldValue(session, payload, ref reader, ref writer, path, true);
            writer.Flush();
            int consumed = checked((int)reader.Consumed);
            if (consumed < payload.Length)
            {
                ReadOnlySpan<byte> trailingPayload = payload.AsSpan(consumed);
                trailingPayload.CopyTo(buffer.GetSpan(trailingPayload.Length));
                buffer.Advance(trailingPayload.Length);
            }

            return buffer.WrittenMemory.ToArray();
        }

        private static bool TryPatchBigWorldNestedNativePayload(Session session, byte[] payload, out byte[] patchedPayload)
        {
            try
            {
                patchedPayload = PatchBigWorldNestedNativePayload(session, payload);
                return true;
            }
            catch
            {
                patchedPayload = payload;
                return false;
            }
        }

        private static void RewriteBigWorldEnterWorldValue(Session session, byte[] payload, ref MessagePackReader reader, ref MessagePackWriter writer, List<string> path, bool patchFixtureIdentityLiterals)
        {
            if (IsBigWorldCommanderPartListPath(path)
                && reader.NextMessagePackType == MessagePackType.Array
                && TryWriteBigWorldCommanderPartList(payload, ref reader, ref writer))
            {
                return;
            }

            if (TryWriteBigWorldEnterWorldReplacement(session, ref writer, path))
            {
                reader.Skip();
                return;
            }

            if (IsBigWorldNestedNativePayloadPath(path) && reader.NextMessagePackType == MessagePackType.Binary)
            {
                ReadOnlySequence<byte>? nestedPayload = reader.ReadBytes();
                writer.Write(PatchBigWorldNestedNativePayload(session, nestedPayload?.ToArray() ?? []));
                return;
            }

            if (patchFixtureIdentityLiterals && reader.NextMessagePackType == MessagePackType.Binary)
            {
                ReadOnlySequence<byte>? nestedPayload = reader.ReadBytes();
                byte[] nestedPayloadBytes = nestedPayload?.ToArray() ?? [];
                writer.Write(TryPatchBigWorldNestedNativePayload(session, nestedPayloadBytes, out byte[] patchedPayload)
                    ? patchedPayload
                    : nestedPayloadBytes);
                return;
            }

            if (patchFixtureIdentityLiterals && TryWriteBigWorldFixtureIdentityReplacement(session, ref reader, ref writer))
                return;

            switch (reader.NextMessagePackType)
            {
                case MessagePackType.Map:
                    int mapCount = reader.ReadMapHeader();
                    writer.WriteMapHeader(mapCount);
                    for (int index = 0; index < mapCount; index++)
                    {
                        string? key = TryReadStringKey(payload, ref reader, ref writer);
                        path.Add(key ?? string.Empty);
                        RewriteBigWorldEnterWorldValue(session, payload, ref reader, ref writer, path, patchFixtureIdentityLiterals);
                        path.RemoveAt(path.Count - 1);
                    }
                    break;
                case MessagePackType.Array:
                    int arrayCount = reader.ReadArrayHeader();
                    writer.WriteArrayHeader(arrayCount);
                    for (int index = 0; index < arrayCount; index++)
                    {
                        path.Add(index.ToString());
                        RewriteBigWorldEnterWorldValue(session, payload, ref reader, ref writer, path, patchFixtureIdentityLiterals);
                        path.RemoveAt(path.Count - 1);
                    }
                    break;
                default:
                    CopyRawMessagePackValue(payload, ref reader, ref writer);
                    break;
            }
        }

        private static string? TryReadStringKey(byte[] payload, ref MessagePackReader reader, ref MessagePackWriter writer)
        {
            if (reader.NextMessagePackType != MessagePackType.String)
            {
                CopyRawMessagePackValue(payload, ref reader, ref writer);
                return null;
            }

            string? key = reader.ReadString();
            writer.Write(key);
            return key;
        }

        private static void CopyRawMessagePackValue(byte[] payload, ref MessagePackReader reader, ref MessagePackWriter writer)
        {
            int start = checked((int)reader.Consumed);
            reader.Skip();
            int length = checked((int)reader.Consumed) - start;
            writer.WriteRaw(payload.AsSpan(start, length));
        }

        private static bool IsBigWorldNestedNativePayloadPath(List<string> path)
        {
            return path.Count == 2
                && path[0] == "EnterResultData"
                && (path[1] == "FightData" || path[1] == "LevelData");
        }

        private static bool TryWriteBigWorldFixtureIdentityReplacement(Session session, ref MessagePackReader reader, ref MessagePackWriter writer)
        {
            try
            {
                (long fixturePlayerId, string fixturePlayerName) = BigWorldFixturePlayer.Value;
                if (reader.NextMessagePackType == MessagePackType.Integer)
                {
                    MessagePackReader probe = reader;
                    long value = probe.ReadInt64();
                    if (value == fixturePlayerId)
                    {
                        reader = probe;
                        WriteBigWorldInteger(ref writer, session.player.PlayerData.Id);
                        return true;
                    }
                }

                if (reader.NextMessagePackType == MessagePackType.String)
                {
                    MessagePackReader probe = reader;
                    string? value = probe.ReadString();
                    if (value == fixturePlayerName)
                    {
                        reader = probe;
                        writer.Write(session.player.PlayerData.Name ?? string.Empty);
                        return true;
                    }
                }
            }
            catch (OverflowException)
            {
            }
            catch (MessagePackSerializationException)
            {
            }
            catch (InvalidOperationException)
            {
            }

            return false;
        }

        private static bool TryWriteBigWorldEnterWorldReplacement(Session session, ref MessagePackWriter writer, List<string> path)
        {
            (int worldId, int levelId) = BigWorldFixtureWorld.Value;

            if (IsBigWorldPlayerPath(path, "Id"))
            {
                WriteBigWorldInteger(ref writer, session.player.PlayerData.Id);
                return true;
            }

            if (IsBigWorldPlayerPath(path, "Name"))
            {
                writer.Write(session.player.PlayerData.Name ?? string.Empty);
                return true;
            }

            if (IsBigWorldPlayerPath(path, "HeadPortraitId"))
            {
                writer.Write((int)session.player.PlayerData.CurrHeadPortraitId);
                return true;
            }

            if (IsBigWorldPlayerPath(path, "HeadFrameId"))
            {
                writer.Write((int)session.player.PlayerData.CurrHeadFrameId);
                return true;
            }

            if (IsBigWorldBornDataPath(path, "LastWorldId") || IsBigWorldPlayerDataPath(path, "LastWorldId"))
            {
                writer.Write(worldId);
                return true;
            }

            if (IsBigWorldBornDataPath(path, "LastLevelId") || IsBigWorldPlayerDataPath(path, "LastLevelId"))
            {
                writer.Write(levelId);
                return true;
            }


            return false;
        }

        private static bool IsBigWorldPlayerPath(List<string> path, string leaf)
        {
            return path.Count == 5
                && path[0] == "EnterResultData"
                && path[1] == "WorldData"
                && path[2] == "Players"
                && path[3] == "0"
                && path[4] == leaf;
        }

        private static bool IsBigWorldBornDataPath(List<string> path, string leaf)
        {
            return path.Count == 6
                && path[0] == "EnterResultData"
                && path[1] == "WorldData"
                && path[2] == "Players"
                && path[3] == "0"
                && path[4] == "BornData"
                && path[5] == leaf;
        }

        private static bool IsBigWorldPlayerDataPath(List<string> path, string leaf)
        {
            return path.Count == 2
                && path[0] == "PlayerData"
                && path[1] == leaf;
        }

        private static void WriteBigWorldInteger(ref MessagePackWriter writer, long value)
        {
            if (value >= int.MinValue && value <= int.MaxValue)
                writer.Write((int)value);
            else
                writer.Write(value);
        }

        private static bool IsBigWorldCommanderPartListPath(List<string> path)
        {
            return path.Count >= 2
                && path[path.Count - 2] == "PartData"
                && path[path.Count - 1] == "PartList";
        }

        private static bool TryWriteBigWorldCommanderPartList(byte[] payload, ref MessagePackReader reader, ref MessagePackWriter writer)
        {
            try
            {
                IReadOnlyList<(int PartId, int ColourId)> fixtureParts = BigWorldFixtureCommanderParts.Value;
                HashSet<int> fixturePartIds = new(fixtureParts.Select(part => part.PartId));
                MessagePackReader probe = reader;
                int count = probe.ReadArrayHeader();
                Dictionary<int, byte[]> rawPartsById = new(count);

                for (int index = 0; index < count; index++)
                {
                    int start = checked((int)probe.Consumed);
                    probe.Skip();
                    int length = checked((int)probe.Consumed) - start;
                    byte[] rawPart = payload.AsSpan(start, length).ToArray();

                    if (TryReadBigWorldPartId(rawPart, out int partId))
                        rawPartsById.TryAdd(partId, rawPart);
                }

                if (rawPartsById.Count == 0
                    || rawPartsById.Count != count
                    || rawPartsById.Keys.Any(partId => !fixturePartIds.Contains(partId))
                    || rawPartsById.Count * 2 < fixtureParts.Count
                    || fixtureParts.All(part => rawPartsById.ContainsKey(part.PartId)))
                {
                    return false;
                }

                reader = probe;
                writer.WriteArrayHeader(fixtureParts.Count);
                foreach ((int partId, int colourId) in fixtureParts)
                {
                    if (rawPartsById.TryGetValue(partId, out byte[]? rawPart))
                        writer.WriteRaw(rawPart);
                    else
                        WriteBigWorldCommanderPart(ref writer, partId, colourId);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool TryReadBigWorldPartId(byte[] partPayload, out int partId)
        {
            partId = 0;
            try
            {
                Dictionary<string, object?> part = MessagePackSerializer.Deserialize<Dictionary<string, object?>>(partPayload);
                if (!part.TryGetValue("PartId", out object? value))
                    return false;

                partId = ReadBigWorldXRpcInt32(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void WriteBigWorldCommanderPart(ref MessagePackWriter writer, int partId, int colourId)
        {
            writer.WriteMapHeader(2);
            writer.Write("PartId");
            writer.Write(partId);
            writer.Write("ColourId");
            writer.Write(colourId);
        }




        internal static void SendPendingBigWorldStartFightNotify(Session session)
        {
            if (session.PendingBigWorldLoadCompleteXRpc)
                session.SendPush(new StartFightNotify());
        }

        internal static void SendPendingBigWorldLoadCompleteXRpc(Session session)
        {
            if (!session.PendingBigWorldLoadCompleteXRpc)
                return;

            session.PendingBigWorldLoadCompleteXRpc = false;
            if (BigWorldLoadCompleteXRpcPushesSnapshot.Value["Pushes"] is not JArray pushes)
                return;

            foreach (JObject push in pushes.OfType<JObject>())
            {
                string? name = push.Value<string>("Name");
                string? payload = push.Value<string>("Payload");
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(payload))
                    continue;

                try
                {
                    byte[] payloadBytes = Convert.FromBase64String(payload);
                    session.SendPush(name, PatchBigWorldXRpcPayload(session, payloadBytes));
                }
                catch
                {
                    // A malformed optional bootstrap fixture entry must not abort LoadComplete handling.
                }
            }
        }

        private static byte[] PatchBigWorldXRpcPayload(Session session, byte[] payload)
        {
            try
            {
                object?[] rpc = MessagePackSerializer.Deserialize<object?[]>(payload);
                if (rpc.Length >= 2
                    && rpc[0] is string { } rpcName
                    && rpcName == "RpcSetCombatState"
                    && rpc[1] is byte[] argsPayload)
                {
                    Dictionary<string, object?> args = MessagePackSerializer.Deserialize<Dictionary<string, object?>>(argsPayload);
                    args["PlayerId"] = checked((int)session.player.PlayerData.Id);
                    rpc[1] = MessagePackSerializer.Serialize(args);
                    return MessagePackSerializer.Serialize(rpc);
                }
            }
            catch
            {
                // Best-effort player-id patching must not block the remaining load-complete RPC bootstrap.
            }

            return payload;
        }

    }
}
