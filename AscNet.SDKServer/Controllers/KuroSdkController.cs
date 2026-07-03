using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AscNet.SDKServer.Controllers
{
    internal class KuroSdkController : IRegisterable
    {
        private const string LocalCuid = "1";
        private const string LocalEmail = "test@ascnet.local";
        private const string LocalUsername = "test";
        private const string LocalToken = "ascnet-local-token";
        private const string LocalOauthCode = "ascnet-local-oauth-code";
        private const string LocalAccessToken = "ascnet-local-access-token";
        private const int SteamLoginType = 23;

        public static void Register(WebApplication app)
        {
            app.MapMethods("/sdkcom/v2/login/emailPwd.lg", ["GET", "POST"], HandleLogin);
            app.MapMethods("/sdkcom/v2/login/third/steam.lg", ["GET", "POST"], HandleLogin);
            app.MapMethods("/sdkcom/v2/login/third/pc/mark.lg", ["GET", "POST"], HandleThirdLoginMark);
            app.MapMethods("/sdkcom/v2/login/third/pc/browser.lg", ["GET", "POST"], HandleThirdLoginBrowser);
            app.MapMethods("/sdkcom/v2/login/auto.lg", ["GET", "POST"], HandleLogin);
            app.MapMethods("/sdkcom/v2/login/real-name/login.lg", ["GET", "POST"], HandleLogin);
            app.MapMethods("/sdkcom/v2/login/preambleCode.lg", ["GET", "POST"], HandleLogin);
            app.MapMethods("/sdkcom/v2/auth/getToken.lg", ["GET", "POST"], HandleAccessToken);
            app.MapMethods("/sdkcom/v2/user/oauth/code/generate.lg", ["GET", "POST"], HandleOauthCode);
            app.MapMethods("/sdkcom/v2/sys/europe/config.lg", ["GET", "POST"], HandleSystemConfig);
            app.MapMethods("/sdkcom/v2/sys/conf.lg", ["GET", "POST"], HandleSystemConfig);
            app.MapGet("/sdkcom/v2/sys/player-config.json", HandlePlayerConfig);
            app.MapMethods("/sdkcom/v2/user/game/role.lg", ["GET", "POST"], HandleOk);
            app.MapMethods("/sdkcom/v2/heartbeat/tokenCheck.lg", ["GET", "POST"], HandleOk);
            app.MapMethods("/sdkcom/v2/bind/device/status.lg", ["GET", "POST"], HandleOk);
            app.MapMethods("/sdkcom/v2/bind/device.lg", ["GET", "POST"], HandleOk);
            app.MapMethods("/sdkcom/v2/real-name-info/check.lg", ["GET", "POST"], HandleRealNameCheck);
            app.MapGet("/sdkcom/v2/local/login", HandleLocalLoginPage);
            app.MapMethods("/sdkcom/v2/local/{**path}", ["GET", "POST"], HandleOk);
        }

        private static IResult HandleLogin(HttpContext ctx)
        {
            return Results.Json(new
            {
                code = 0,
                msg = "success",
                data = LoginData(ctx)
            });
        }

        private static IResult HandleAccessToken(HttpContext ctx)
        {
            return Results.Json(new
            {
                code = 0,
                msg = "success",
                access_token = LocalAccessToken,
                expires_in = 2592000,
                data = new
                {
                    access_token = LocalAccessToken,
                    expires_in = 2592000
                }
            });
        }

        private static IResult HandleOauthCode(HttpContext ctx)
        {
            return Results.Json(new
            {
                code = 0,
                msg = "success",
                data = new
                {
                    oauthCode = LocalOauthCode,
                    code = LocalOauthCode
                }
            });
        }

        private static IResult HandleThirdLoginMark(HttpContext ctx)
        {
            string mark = RequestValue(ctx, "mark") ?? "ascnet-local-third-login";
            return Results.Json(new
            {
                code = 0,
                msg = "success",
                data = new
                {
                    ready = 1,
                    mark
                }
            });
        }

        private static IResult HandleThirdLoginBrowser(HttpContext ctx)
        {
            string origin = LocalSdkOrigin(ctx);
            string mark = RequestValue(ctx, "mark") ?? "ascnet-local-third-login";
            string type = RequestValue(ctx, "type", "loginType") ?? SteamLoginType.ToString();
            string url = $"{origin}/sdkcom/v2/local/login?mark={Uri.EscapeDataString(mark)}&type={Uri.EscapeDataString(type)}";
            return Results.Json(new
            {
                code = 0,
                msg = "success",
                data = url,
                url
            });
        }

        private static IResult HandleSystemConfig(HttpContext ctx)
        {
            return Results.Json(new
            {
                code = 0,
                msg = "success",
                data = PlayerConfigData(ctx)
            });
        }

        private static IResult HandlePlayerConfig(HttpContext ctx)
        {
            return Results.Json(PlayerConfigData(ctx));
        }

        private static IResult HandleLocalLoginPage(HttpContext ctx)
        {
            string origin = LocalSdkOrigin(ctx);
            string gateUrl = $"{origin}/api/Login/Login";
            string html = $$"""
<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>AscNet Local Login</title>
  <style>
    :root { color-scheme: dark; font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif; background: #0f1117; color: #f6f7fb; }
    body { margin: 0; min-height: 100vh; display: grid; place-items: center; }
    main { width: min(520px, calc(100vw - 48px)); padding: 32px; border: 1px solid #2d3342; border-radius: 18px; background: #171b25; box-shadow: 0 18px 48px rgba(0,0,0,.35); }
    h1 { margin: 0 0 12px; font-size: 26px; }
    p { line-height: 1.55; color: #cbd2e1; }
    code { color: #9ae6b4; word-break: break-all; }
    .ok { margin-top: 18px; padding: 14px 16px; border-radius: 12px; background: #10291d; color: #a7f3d0; }
  </style>
</head>
<body>
  <main>
    <h1>AscNet local login is active</h1>
    <p>The Steam KRSDK bridge is routed to this AscNet instance.</p>
    <p>Create or select the local AscNet account with <code>run_steam.py --ascnet-username</code>; the bridge maps successful Steam/KRSDK login callbacks to that account at <code>{{WebUtility.HtmlEncode(gateUrl)}}</code>.</p>
    <div class="ok">Close this window and press login again after the Steam/KRSDK account has authenticated.</div>
  </main>
</body>
</html>
""";
            return Results.Content(html, "text/html");
        }

        private static Dictionary<string, object> PlayerConfigData(HttpContext ctx)
        {
            string origin = LocalSdkOrigin(ctx);
            string playerConfigUrl = $"{origin}/sdkcom/v2/sys/player-config.json";
            string pcGeetestUrl = $"{origin}/sdkcom/v2/local/geetest";
            string accCenterUrl = $"{origin}/sdkcom/v2/local/account-center";
            string pcThirdLoginUrl = $"{origin}/sdkcom/v2/local/login";
            string customerServiceUrl = $"{origin}/sdkcom/v2/local/customer-service";
            string sobotRedDotUrl = $"{origin}/sdkcom/v2/local/sobot/red-dot";
            string googlePcAuthResultUrl = $"{origin}/sdkcom/v2/local/google-pc-auth-result";
            string emailSystemUrl = $"{origin}/sdkcom/v2/local/email-system";
            string bizsirenUrl = $"{origin}/sdkcom/v2/local/bizsiren";
            string clientUrl = JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                ["pcGeetestUrl"] = pcGeetestUrl,
                ["accCenterUrl"] = accCenterUrl,
                ["pcThirdLoginUrl"] = pcThirdLoginUrl,
                ["kefu"] = customerServiceUrl,
                ["kefuServ"] = customerServiceUrl,
                ["sobot"] = 1,
                ["sobotRedDotUrl"] = sobotRedDotUrl,
                ["googlePcAuthResultUrl"] = googlePcAuthResultUrl,
                ["emailSystemUrl"] = emailSystemUrl,
                ["bizsiren"] = bizsirenUrl
            });

            return new Dictionary<string, object>
            {
                ["link"] = new[] { new { url = playerConfigUrl, weight = 1 } },
                ["clientSwitch"] = 1,
                ["ageCheckBox"] = 0,
                ["regionMinAge"] = 0,
                ["googlePcConsumeIntervalSec"] = 60,
                ["audioSwitch"] = 1,
                ["kefuSessionExpireHour"] = 1,
                ["ignoreCheckWndVisibleForWin"] = 0,
                ["enableTransparentJiYanWebviewForWin"] = 0,
                ["clientFocus"] = 1,
                ["krData"] = 0,
                ["marketAppsFlyer"] = 0,
                ["pcGeetestUrl"] = pcGeetestUrl,
                ["accCenterUrl"] = accCenterUrl,
                ["clientUrl"] = clientUrl,
                ["pcThirdLoginUrl"] = pcThirdLoginUrl,
                ["thirdLogin"] = 1,
                ["heartFreq"] = 60,
                ["kefuInterval"] = 60,
                ["kefu"] = customerServiceUrl,
                ["kefuServ"] = customerServiceUrl,
                ["sobot"] = 1,
                ["sobotRedDotUrl"] = sobotRedDotUrl,
                ["googlePcAuthResultUrl"] = googlePcAuthResultUrl,
                ["emailSystemUrl"] = emailSystemUrl,
                ["bizsiren"] = bizsirenUrl,
                ["webviewIgnoreCertErrorForWin"] = 1,
                ["disableAgreementRemainDlgForWin"] = 1,
                ["PayEventFixedForPCSteam"] = 1,
                ["clientFocusForSteamDeck"] = 1,
                ["useGoogleSDKForWin"] = 1,
                ["disableWebviewVideoSupportedForWin"] = 0,
                ["useNewClientFocus"] = 0,
                ["enableTransparentJiYanWebview"] = 0
            };
        }

        private static string LocalSdkOrigin(HttpContext ctx)
        {
            string? configuredOrigin = Environment.GetEnvironmentVariable("ASCNET_PUBLIC_HTTP_ORIGIN")?.TrimEnd('/');
            if (!string.IsNullOrEmpty(configuredOrigin))
                return configuredOrigin;

            string scheme = ctx.Request.Headers["X-Forwarded-Proto"].FirstOrDefault() ?? ctx.Request.Scheme;
            string host = ctx.Request.Headers["X-Forwarded-Host"].FirstOrDefault() ?? ctx.Request.Host.Value;
            return $"{scheme}://{host}".TrimEnd('/');
        }


        private static IResult HandleRealNameCheck(HttpContext ctx)
        {
            return Results.Json(new
            {
                code = 0,
                msg = "success",
                data = new
                {
                    realNameMethod = 0,
                    realNameKey = string.Empty,
                    realNameUrl = string.Empty
                }
            });
        }

        private static IResult HandleOk(HttpContext ctx)
        {
            return Results.Json(new
            {
                code = 0,
                msg = "success",
                data = new { }
            });
        }

        private static object LoginData(HttpContext ctx)
        {
            string cuid = RequestValue(ctx, "cuid", "uid", "userId", "id") ?? LocalCuid;
            string username = RequestValue(ctx, "username") ?? LocalUsername;
            string email = RequestValue(ctx, "email") ?? LocalEmail;
            string token = RequestValue(ctx, "token", "autoToken", "accessToken") ?? LocalToken;
            int loginType = RequestIntValue(ctx, "loginType") ?? SteamLoginType;
            long id = RequestLongValue(ctx, "cuid", "uid", "userId", "id") ?? 1;

            return new
            {
                id,
                cuid,
                username,
                loginType,
                code = RequestValue(ctx, "code") ?? "0",
                email,
                autoToken = token,
                token,
                bindDevStat = 0,
                idStat = 0,
                firstLgn = 0,
                bindDevMsg = string.Empty,
                realNameMethod = 0,
                thirdNickName = RequestValue(ctx, "thirdNickName", "nickname", "nickName") ?? username,
                bindDevSwitch = 0,
                realNameUrl = string.Empty,
                realNameKey = string.Empty
            };
        }

        private static string? FirstQueryValue(HttpContext ctx, string key)
        {
            return ctx.Request.Query.TryGetValue(key, out var values) ? values.FirstOrDefault() : null;
        }

        private static string? RequestValue(HttpContext ctx, params string[] keys)
        {
            foreach (string key in keys)
            {
                string? queryValue = FirstQueryValue(ctx, key);
                if (!string.IsNullOrEmpty(queryValue))
                    return queryValue;
            }

            if (!ctx.Request.HasFormContentType)
                return null;

            IFormCollection form = ctx.Request.ReadFormAsync().GetAwaiter().GetResult();
            foreach (string key in keys)
            {
                if (form.TryGetValue(key, out var values))
                {
                    string? value = values.FirstOrDefault();
                    if (!string.IsNullOrEmpty(value))
                        return value;
                }
            }

            return null;
        }

        private static int? RequestIntValue(HttpContext ctx, params string[] keys)
        {
            string? value = RequestValue(ctx, keys);
            return int.TryParse(value, out int parsed) ? parsed : null;
        }

        private static long? RequestLongValue(HttpContext ctx, params string[] keys)
        {
            string? value = RequestValue(ctx, keys);
            return long.TryParse(value, out long parsed) ? parsed : null;
        }
    }
}
