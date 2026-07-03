using AscNet.Common.Util;
using AscNet.SDKServer.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AscNet.SDKServer.Controllers
{
    internal class ConfigController : IRegisterable
    {
        private static readonly Dictionary<string, ServerVersionConfig> versions = new();

        static ConfigController()
        {
            versions = JsonConvert.DeserializeObject<Dictionary<string, ServerVersionConfig>>(File.ReadAllText("./Configs/version_config.json"))!;
        }

        public static void Register(WebApplication app)
        {
            app.MapGet("/prod/client/config/{package}/{version}/standalone/config.tab", HandleConfigRequest);
            app.MapGet("/prod/client/config/{cdnKey}/{package}/{version}/standalone/config.tab", HandleConfigRequest);

            app.MapGet("/prod/client/notice/config/{package}/{version}/LoginNotice.json", HandleLoginNoticeRequest);
            app.MapGet("/prod/client/notice/config/{cdnKey}/{package}/{version}/LoginNotice.json", HandleLoginNoticeRequest);
            app.MapGet("/prod/client/notice/{package}/{version}/standalone/LoginNotice.json", HandleLoginNoticeRequest);
            app.MapGet("/prod/client/notice/{cdnKey}/{package}/{version}/standalone/LoginNotice.json", HandleLoginNoticeRequest);
            app.MapGet("/prod/client/notice/config/{package}/{version}/ScrollTextNotice.json", HandleScrollTextNoticeRequest);
            app.MapGet("/prod/client/notice/config/{cdnKey}/{package}/{version}/ScrollTextNotice.json", HandleScrollTextNoticeRequest);
            app.MapGet("/prod/client/notice/{package}/{version}/standalone/ScrollTextNotice.json", HandleScrollTextNoticeRequest);
            app.MapGet("/prod/client/notice/{cdnKey}/{package}/{version}/standalone/ScrollTextNotice.json", HandleScrollTextNoticeRequest);
            app.MapGet("/prod/client/notice/config/{package}/{version}/ScrollPicNotice.json", HandleScrollPicNoticeRequest);
            app.MapGet("/prod/client/notice/config/{cdnKey}/{package}/{version}/ScrollPicNotice.json", HandleScrollPicNoticeRequest);
            app.MapGet("/prod/client/notice/{package}/{version}/standalone/ScrollPicNotice.json", HandleScrollPicNoticeRequest);
            app.MapGet("/prod/client/notice/{cdnKey}/{package}/{version}/standalone/ScrollPicNotice.json", HandleScrollPicNoticeRequest);
            app.MapGet("/prod/client/notice/config/{package}/{version}/GameNotice.json", HandleGameNoticeRequest);
            app.MapGet("/prod/client/notice/config/{cdnKey}/{package}/{version}/GameNotice.json", HandleGameNoticeRequest);
            app.MapGet("/prod/client/notice/{package}/{version}/standalone/GameNotice.json", HandleGameNoticeRequest);
            app.MapGet("/prod/client/notice/{cdnKey}/{package}/{version}/standalone/GameNotice.json", HandleGameNoticeRequest);
            app.MapGet("/prod/client/notice/config/{package}/{version}/SecondMenuNotice.json", HandleSecondMenuNoticeRequest);
            app.MapGet("/prod/client/notice/config/{cdnKey}/{package}/{version}/SecondMenuNotice.json", HandleSecondMenuNoticeRequest);
            app.MapGet("/prod/client/notice/{package}/{version}/standalone/SecondMenuNotice.json", HandleSecondMenuNoticeRequest);
            app.MapGet("/prod/client/notice/{cdnKey}/{package}/{version}/standalone/SecondMenuNotice.json", HandleSecondMenuNoticeRequest);
            app.MapGet("/prod/client/notice/config/{package}/{version}/PopUpPicNotice.json", HandlePopUpPicNoticeRequest);
            app.MapGet("/prod/client/notice/config/{cdnKey}/{package}/{version}/PopUpPicNotice.json", HandlePopUpPicNoticeRequest);
            app.MapGet("/prod/client/notice/{package}/{version}/standalone/PopUpPicNotice.json", HandlePopUpPicNoticeRequest);
            app.MapGet("/prod/client/notice/{cdnKey}/{package}/{version}/standalone/PopUpPicNotice.json", HandlePopUpPicNoticeRequest);


            app.MapPost("/feedback", (HttpContext ctx) =>
            {
                SDKServer.log.Info("1");
                return "1";
            });
        }

        private static string HandleConfigRequest(HttpContext ctx)
        {
            string package = GetRouteValue(ctx, "package");
            string version = GetRouteValue(ctx, "version");
            bool currentClient = IsVersionAtLeast(version, 4, 5, 0);
            string publicHttpOrigin = PublicHttpOrigin(ctx);
            ServerVersionConfig versionConfig = GetVersionConfig(version);

            List<RemoteConfig> remoteConfigs = new();
            if (currentClient)
                AddCurrentClientConfig(remoteConfigs, package, version, versionConfig, publicHttpOrigin);
            else
                AddLegacyClientConfig(remoteConfigs, package, version, versionConfig, publicHttpOrigin);

            string serializedObject = TsvTool.SerializeObject(remoteConfigs);
            SDKServer.log.Info(serializedObject);
            return serializedObject;
        }

        private static string HandleLoginNoticeRequest(HttpContext ctx)
        {
            if (TryReadNoticeFixture(ctx, "LoginNotice.json", out string fixtureJson))
                return fixtureJson;

            LoginNotice notice = new()
            {
                BeginTime = 0,
                EndTime = 0,
                HtmlUrl = "/",
                Id = "1",
                ModifyTime = DateTimeOffset.Now.ToUnixTimeSeconds(),
                Title = "NOTICE",
                LoginPlatformList = DefaultLoginPlatforms()
            };

            return SerializeAndLog(notice);
        }

        private static string HandleScrollTextNoticeRequest(HttpContext ctx)
        {
            if (TryReadNoticeFixture(ctx, "ScrollTextNotice.json", out string fixtureJson))
                return fixtureJson;

            ScrollTextNotice notice = new()
            {
                Id = "1",
                ModifyTime = DateTimeOffset.Now.ToUnixTimeSeconds(),
                BeginTime = 0,
                EndTime = 0,
                Content = "[ANNOUNCEMENT] There is no announcement.",
                ScrollInterval = 300,
                ScrollTimes = 15,
                ShowInFight = 1,
                ShowInPhotograph = 1,
                LoginPlatformList = DefaultLoginPlatforms()
            };

            return SerializeAndLog(notice);
        }

        private static string HandleScrollPicNoticeRequest(HttpContext ctx)
        {
            if (TryReadNoticeFixture(ctx, "ScrollPicNotice.json", out string fixtureJson))
                return fixtureJson;

            ScrollPicNotice notice = new()
            {
                Id = "1",
                ModifyTime = DateTimeOffset.Now.ToUnixTimeSeconds(),
                Content =
                [
                    new ScrollPicNotice.NoticeContent()
                    {
                        Id = 0,
                        PicAddr = "0",
                        JumpType = "0",
                        JumpAddr = "0",
                        PicType = "0",
                        Interval = 5,
                        BeginTime = DateTimeOffset.Now.ToUnixTimeSeconds(),
                        EndTime = DateTimeOffset.Now.ToUnixTimeSeconds() + 3600 * 24,
                        AppearanceCondition = Array.Empty<dynamic>(),
                        AppearanceDay = Array.Empty<dynamic>(),
                        AppearanceTime = Array.Empty<dynamic>(),
                        DisappearanceCondition = Array.Empty<dynamic>(),
                    }
                ],
                LoginPlatformList = DefaultLoginPlatforms()
            };

            return SerializeAndLog(notice);
        }

        private static string HandleGameNoticeRequest(HttpContext ctx)
        {
            if (TryReadNoticeFixture(ctx, "GameNotice.json", out string fixtureJson))
                return fixtureJson;

            List<GameNotice> notices = new();
            return SerializeAndLog(notices);
        }

        private static string HandleSecondMenuNoticeRequest(HttpContext ctx)
        {
            if (TryReadNoticeFixture(ctx, "SecondMenuNotice.json", out string fixtureJson))
                return fixtureJson;

            return SerializeAndLog(new
            {
                Id = "ascnet-empty-second-menu",
                ModifyTime = DateTimeOffset.Now.ToUnixTimeSeconds(),
                Content = Array.Empty<object>(),
                LoginPlatformList = DefaultLoginPlatforms()
            });
        }

        private static string HandlePopUpPicNoticeRequest(HttpContext ctx)
        {
            if (TryReadNoticeFixture(ctx, "PopUpPicNotice.json", out string fixtureJson))
                return fixtureJson;

            return SerializeAndLog(new
            {
                Id = "ascnet-empty-popup-pic",
                ModifyTime = DateTimeOffset.Now.ToUnixTimeSeconds(),
                Content = Array.Empty<object>(),
                LoginPlatformList = DefaultLoginPlatforms()
            });
        }

        private static void AddLegacyClientConfig(List<RemoteConfig> remoteConfigs, string package, string version, ServerVersionConfig versionConfig, string publicHttpOrigin)
        {
            (string primaryCdns, string secondaryCdns, int channel) = GetPackageConfig(package, currentClient: false);

            remoteConfigs.AddConfig("DocumentVersion", versionConfig.DocumentVersion);
            remoteConfigs.AddConfig("LaunchModuleVersion", versionConfig.LaunchModuleVersion);
            remoteConfigs.AddConfig("IndexMd5", versionConfig.IndexMd5);
            remoteConfigs.AddConfig("IndexSha1", versionConfig.IndexSha1);
            remoteConfigs.AddConfig("LaunchIndexSha1", versionConfig.LaunchIndexSha1);
            remoteConfigs.AddConfig("ApplicationVersion", version);
            remoteConfigs.AddConfig("Debug", true);
            remoteConfigs.AddConfig("External", true);
            remoteConfigs.AddConfig("PayCallbackUrl", $"{publicHttpOrigin}/api/XPay/KuroPayResult");
            remoteConfigs.AddConfig("PrimaryCdns", primaryCdns);
            remoteConfigs.AddConfig("SecondaryCdns", secondaryCdns);
            remoteConfigs.AddConfig("Channel", channel);
            remoteConfigs.AddConfig("CdnInvalidTime", 60);
            remoteConfigs.AddConfig("MtpEnabled", false);
            remoteConfigs.AddConfig("MemoryLimit", 2048);
            remoteConfigs.AddConfig("CloseMsgEncrypt", false);
            remoteConfigs.AddConfig("ServerListStr", $"{Common.Common.config.GameServer.RegionName}#{publicHttpOrigin}/api/Login/Login");
            remoteConfigs.AddConfig("AndroidPayCallbackUrl", $"{publicHttpOrigin}/api/XPay/HeroHgAndroidPayResult");
            remoteConfigs.AddConfig("IosPayCallbackUrl", $"{publicHttpOrigin}/api/XPay/HeroHgIosPayResult");
            remoteConfigs.AddConfig("WatermarkEnabled", false);
            remoteConfigs.AddConfig("PicComposition", "empty");
            remoteConfigs.AddConfig("DeepLinkEnabled", true);
            remoteConfigs.AddConfig("DownloadMethod", 1);
            remoteConfigs.AddConfig("PcPayCallbackList", $"{publicHttpOrigin}/api/XPay/KuroPayResult");
            remoteConfigs.AddConfig("WatermarkType", 2);
            remoteConfigs.AddConfig("ChannelServerListStr", $"1#{Common.Common.config.GameServer.RegionName}#{publicHttpOrigin}/api/Login/Login");
        }

        private static void AddCurrentClientConfig(List<RemoteConfig> remoteConfigs, string package, string version, ServerVersionConfig versionConfig, string publicHttpOrigin)
        {
            (string primaryCdns, string secondaryCdns, int channel) = GetPackageConfig(package, currentClient: true);

            remoteConfigs.AddConfig("ApplicationVersion", version);
            remoteConfigs.AddConfig("DocumentVersion", versionConfig.DocumentVersion);
            remoteConfigs.AddConfig("Debug", false);
            remoteConfigs.AddConfig("External", true);
            remoteConfigs.AddConfig("Channel", channel);
            remoteConfigs.AddConfig("PayCallbackUrl", $"{publicHttpOrigin}/api/XPay/KuroPayResult");
            remoteConfigs.AddConfig("KuroPayCallbackUrl", $"{publicHttpOrigin}/api/XPay/KuroPayResult");
            remoteConfigs.AddConfig("PrimaryCdns", primaryCdns);
            remoteConfigs.AddConfig("SecondaryCdns", secondaryCdns);
            remoteConfigs.AddConfig("CdnInvalidTime", 600);
            remoteConfigs.AddConfig("MtpEnabled", true);
            remoteConfigs.AddConfig("MemoryLimit", 2048);
            remoteConfigs.AddConfig("CloseMsgEncrypt", false);
            remoteConfigs.AddConfig("ServerListStr", CurrentServerListStr(publicHttpOrigin));
            remoteConfigs.AddConfig("IndexMd5", versionConfig.IndexMd5);
            remoteConfigs.AddConfig("AndroidReturnEnabled", false);
            remoteConfigs.AddConfig("AndroidPayCallbackList", $"{publicHttpOrigin}/api/XPay/HeroHgAndroidPayResult");
            remoteConfigs.AddConfig("AndroidPayCallbackUrl", $"{publicHttpOrigin}/api/XPay/HeroHgAndroidPayResult");
            remoteConfigs.AddConfig("IosPayCallbackUrl", $"{publicHttpOrigin}/api/XPay/HeroHgIosPayResult");
            remoteConfigs.AddConfig("DEEnable", true);
            remoteConfigs.AddConfig("DEFilter", "empty");
            remoteConfigs.AddConfig("IndexSha1", versionConfig.IndexSha1);
            remoteConfigs.AddConfig("WatermarkEnabled", false);
            remoteConfigs.AddConfig("PicComposition", "empty");
            remoteConfigs.AddConfig("IosPayCallbackList", $"{publicHttpOrigin}/api/XPay/HeroHgIosPayResult");
            remoteConfigs.AddConfig("LaunchModuleVersion", versionConfig.LaunchModuleVersion);
            remoteConfigs.AddConfig("LaunchIndexSha1", versionConfig.LaunchIndexSha1);
            remoteConfigs.AddConfig("DeepLinkEnabled", true);
            remoteConfigs.AddConfig("AccountCancellationEnable", false);
            remoteConfigs.AddConfig("DownloadMethod", 1);
            remoteConfigs.AddConfig("PcPayCallbackList", $"{publicHttpOrigin}/api/XPay/KuroPayResult");
            remoteConfigs.AddConfig("PcPayCallbackUrl", $"{publicHttpOrigin}/api/XPay/KuroPayResult");
            remoteConfigs.AddConfig("ParallelDownload", 1);
            remoteConfigs.AddConfig("ParallelQueueSize", "3-7");
            remoteConfigs.AddConfig("WatermarkType", 0);
            remoteConfigs.AddConfig("IsPCPayEnable", true);
            remoteConfigs.AddConfig("ChannelServerListStr", CurrentChannelServerListStr(publicHttpOrigin));
            remoteConfigs.AddConfig("IsHeXie", false);
            remoteConfigs.AddConfig("UsingXTableBehaviorNodeOptimize", true);
            remoteConfigs.AddConfig("IsUsingCDNAuth", false);
            remoteConfigs.AddConfig("AuthSignName", "sign");
            remoteConfigs.AddConfig("AuthTimeOut", 1800);
            remoteConfigs.AddConfig("AuthIsVolKey", "volcdn");
        }

        private static string CurrentServerListStr(string publicHttpOrigin)
        {
            string loginUrl = $"{publicHttpOrigin}/api/Login/Login";
            return $"NorthAmerica#{loginUrl}|Europe#{loginUrl}|Asia-Pacific#{loginUrl}";
        }

        private static string CurrentChannelServerListStr(string publicHttpOrigin)
        {
            return $"default#Globle#{publicHttpOrigin}/api/Login/Login";
        }

        private static (string PrimaryCdns, string SecondaryCdns, int Channel) GetPackageConfig(string package, bool currentClient)
        {
            return package switch
            {
                "com.kurogame.haru.kuro" => (
                    "http://prod-zspnsalicdn.kurogame.com/prod",
                    "http://prod-zspnstxcdn.kurogame.com/prod",
                    2),
                "com.kurogame.punishing.grayraven.en" or "com.kurogame.gplay.punishing.grayraven.en" when currentClient => (
                    "http://prod-encdn-tx.kurogame.net/prod",
                    "http://prod-encdn-aliyun.kurogame.net/prod",
                    5),
                "com.kurogame.pc.punishing.grayraven.en" when currentClient => (
                    "http://prod-encdn-tx.kurogame.net/prod",
                    "http://prod-encdn-aliyun.kurogame.net/prod",
                    205),
                _ => (
                    "http://prod-encdn-akamai.kurogame.net/prod|http://prod-encdn-aliyun.kurogame.net/prod",
                    "http://prod-encdn-aliyun.kurogame.net/prod",
                    1)
            };
        }

        private static string? FirstForwardedValue(Microsoft.Extensions.Primitives.StringValues values)
        {
            if (values.Count < 1 || string.IsNullOrWhiteSpace(values[0]))
                return null;

            return values[0]!.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        }

        private static string PublicHttpOrigin(HttpContext ctx)
        {
            string scheme = FirstForwardedValue(ctx.Request.Headers["X-Forwarded-Proto"]) ?? ctx.Request.Scheme;
            string host = FirstForwardedValue(ctx.Request.Headers["X-Forwarded-Host"]) ?? ctx.Request.Host.Value;
            return $"{scheme}://{host}".TrimEnd('/');
        }


        private static ServerVersionConfig GetVersionConfig(string version)
        {
            if (versions.TryGetValue(version, out ServerVersionConfig? versionConfig))
                return versionConfig;

            ServerVersionConfig? latestConfig = null;
            Version? latestVersion = null;

            foreach (var knownVersion in versions)
            {
                if (!Version.TryParse(knownVersion.Key, out Version? parsedVersion))
                    continue;

                if (latestVersion is null || parsedVersion.CompareTo(latestVersion) > 0)
                {
                    latestVersion = parsedVersion;
                    latestConfig = knownVersion.Value;
                }
            }

            return latestConfig ?? versions.First().Value;
        }

        private static bool IsVersionAtLeast(string version, int major, int minor, int patch)
        {
            return Version.TryParse(version, out Version? parsedVersion)
                && parsedVersion.CompareTo(new Version(major, minor, patch)) >= 0;
        }

        private static string GetRouteValue(HttpContext ctx, string key)
        {
            return (string)ctx.Request.RouteValues[key]!;
        }

        private static bool TryReadNoticeFixture(HttpContext ctx, string fileName, out string fixtureJson)
        {
            string version = GetRouteValue(ctx, "version");
            string path = Path.Combine("Configs", "Notices", version, fileName);
            if (!File.Exists(path))
            {
                fixtureJson = string.Empty;
                return false;
            }

            fixtureJson = File.ReadAllText(path);
            SDKServer.log.Info(fixtureJson);
            return true;
        }

        private static int[] DefaultLoginPlatforms()
        {
            return [0, 1, 2];
        }

        private static string SerializeAndLog(object value)
        {
            string serializedObject = JsonConvert.SerializeObject(value);
            SDKServer.log.Info(serializedObject);
            return serializedObject;
        }
    }
}
