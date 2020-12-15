using PuppeteerSharp;
using System;
using System.Threading.Tasks;
using System.Web;

namespace HeatMapExe
{
    public class WebDriver
    {
        public async Task StartAsync()
        {
            Console.WriteLine("Using Browser to get Authorization Code from Strava");

            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            Console.WriteLine("Got Chrome Driver");

            // Create an instance of the browser and configure launch options
            Browser browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Timeout = (int)TimeSpan.FromSeconds(10).TotalMilliseconds
            });
            Console.WriteLine("Created Browser Instance");

            try
            {

                // Create a new page and go to Bing Maps
                using (var page = await browser.NewPageAsync())
                {
                    
                    var response = await page.GoToAsync("http://www.strava.com/oauth/authorize?client_id=9912&response_type=code&redirect_uri=http://localhost/exchange_token&approval_prompt=auto&scope=profile:read_all,profile:write,activity:write,activity:read_all");
                    Console.WriteLine("Logging In");
                    await LoginPageAsync(page);
                    Console.WriteLine("Logged In");
                    Console.WriteLine("Authorization");
                    await AuthorizePageAsync(page);
                }
            }
            catch (Exception)
            {
                Console.ReadKey();
                throw;
            }
            finally
            {
                await browser.CloseAsync();
                await browser.DisposeAsync();
            }
        }

        private async Task LoginPageAsync(Page page)
        {
            await EnterTextAsync(page, "#email", "martingay1975@googlemail.com");
            await EnterTextAsync(page, "#password", "weston2184Strava.");
            await HitButtonAsync(page, ".btn-accept-cookie-banner");
            await HitButtonAsync(page, "#login-button");
        }

        private async Task AuthorizePageAsync(Page page)
        {
            page.RequestFailed += Page_RequestFailed;
            await HitButtonAsync(page, "#authorize");
        }

        private void Page_RequestFailed(object sender, RequestEventArgs e)
        {
            Console.WriteLine($"{e.Request.Url}");

            // e.g. http://localhost/exchange_token?state=&code=8e8df3e9b313d18981095d7f18ab8dffaaf75344&scope=read,activity:write,activity:read_all,profile:write,profile:read_all
            var code = HttpUtility.ParseQueryString(e.Request.Url).Get("code");
            if (!string.IsNullOrWhiteSpace(code))
            {
                OnAutorizationCodeObtained(new AutorizationCodeObtainedEventArgs { Code = code });
            }
        }

        private async Task HitButtonAsync(Page page, string selector, bool wait = true)
        {
            await page.WaitForSelectorAsync(selector);
            await page.ClickAsync(selector);

            if (wait)
            {
                await Task.Delay(500);
            }
        }

        private async Task EnterTextAsync(Page page, string selector, string value)
        {
            await page.WaitForSelectorAsync(selector);
            await page.FocusAsync(selector);
            await page.Keyboard.TypeAsync(value);
        }


        protected virtual void OnAutorizationCodeObtained(AutorizationCodeObtainedEventArgs e)
        {
            AutorizationCodeObtainedEventHandler handler = AutorizationCodeObtained;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public delegate void AutorizationCodeObtainedEventHandler(object sender, AutorizationCodeObtainedEventArgs e);
        public event AutorizationCodeObtainedEventHandler AutorizationCodeObtained;
    }

    public class AutorizationCodeObtainedEventArgs : EventArgs
    {
        public string Code { get; set; }
    }
}
