using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Scrapping.Models;

namespace Scrapping
{
    public  class WebServerExport : IDisposable
    {
        /* 
         * Preparacoes iniciais do webdrive
         */
        private SeleniumConfig _config;
        private IWebDriver _driver;


        public WebServerExport(SeleniumConfig config)
        {
            // Pega config do appsettings
            this._config = config;
            ChromeOptions optionsFF = new ChromeOptions();
            // Define local do chromedrive.exe para a pasta de execucao do executavel
            var chromeDriverService = ChromeDriverService.CreateDefaultService(AppContext.BaseDirectory);

            // Desativa mensagens de debug do chromedrive
            chromeDriverService.HideCommandPromptWindow = true;
            chromeDriverService.SuppressInitialDiagnosticInformation = true;

            // Configuracoes de visualizacao do crhome
            optionsFF.AddArgument("--start-maximized");
            optionsFF.AddArgument("--disable-gpu");
            optionsFF.AddArgument("--disable-extensions");
            optionsFF.AddArgument("--window-size=1920,1080");

            // Definicao da oasta de download, no caso atribuindo para a pasta do executavel
            optionsFF.AddUserProfilePreference("download.default_directory", AppContext.BaseDirectory);

            // Esconder o chrome dependendo do valor do appsettings
            if (_config.Headless == 1)
                optionsFF.AddArgument("--headless");

            // Inicia chrome de teste
            _driver = new ChromeDriver(
                chromeDriverService
                , optionsFF);

            // Navega para a pagina inicial
            _driver.Navigate().GoToUrl(_config.UrlPagina);
        }

        /* 
         * Iniciar a automacao
         */
        public void InitExport()
        {
            // Clique do menu Fetch
            ClickFetch();

            // Realiza ações de click nos menus de proximo/anterior
            ClickNextPage();
            ClickNextPage();
            ClickPreviousPage();


            // Clique do menu FetchAlert
            ClickFetchAlert();
            ClickNextPage(true);
            ClickPreviousPage(true);

            // Clique do menu FetchDownload
            ClickFetchDownload();
            ClickDownload();


            // Clique do menu Fetchdownload
            ClickFetchLogin();

            // Envia dados de autenticacao
            SendAuthentication();
            // Força um pausa para realizacao do Captcha
            Console.WriteLine("Pressione enter apos resolver o captcha e realizar o login.");
            Console.ReadLine();
            ClickNextPage();
            ClickNextPage();
            ClickPreviousPage();


            // Realiza automação de pegar informações
            ClickFetch();

            getInfos();
            Console.ReadLine();

        }

        /*
         * Realizar o fechamento do webdriver
         */
        public void Dispose()
        {
            // Forca encerrar o chromedrive
            _driver.Quit();
            _driver = null;
        }

        /*
         * Fetch comum
         */
        public void ClickFetch()
        {
            XPathClick("//*[@id=\"app\"]/div/div/div[2]/nav/div[3]/a");
        }

        /*
         * Fetch com Alert
         */
        public void ClickFetchAlert()
        {
            XPathClick("//*[@id=\"app\"]/div/div/div[2]/nav/div[4]/a");
        }

        /* 
         * Aceitar alert
         */
        public void ClickAlert()
        {
            _driver.SwitchTo().Alert().Accept();
        }

        /*
         * Fetch com Download
         */
        public void ClickFetchDownload()
        {
            XPathClick("//*[@id=\"app\"]/div/div/div[2]/nav/div[5]/a");
        }

        /*
         * Fetch com Login
         */
        public void ClickFetchLogin()
        {
            XPathClick("//*[@id=\"app\"]/div/div/div[2]/nav/div[6]/a");
        }

        /*
         * Proxima paginacao
         */
        public void ClickNextPage(bool hasAlert = false)
        {
            XPathClick("//button[contains(text(),'Proxima')]");
            if (hasAlert)
                ClickAlert();
        }

        /*
         * Voltar paginacao
         */
        public void ClickPreviousPage(bool hasAlert = false)
        {
            XPathClick("//button[contains(text(),'Anterior')]");
            if (hasAlert)
                ClickAlert();
        }

        /*
         * Download dos dados
         */
        public void ClickDownload()
        {
            XPathClick("//*[@id=\"app\"]/div/main/article/a");
        }

        /*
         * Enviar dados de autenticação
         */
        public void SendAuthentication()
        {
            var email = _driver.FindElement(By.XPath("//*[@id=\"typeEmailX\"]"), _config.Timeout);
            var password = _driver.FindElement(By.XPath("//*[@id=\"typePasswordX\"]"), _config.Timeout);

            email.SendKeys(_config.Username);
            password.SendKeys(_config.Password);

        }

        /*
         * Pegar informacoes
         */
        public void getInfos()
        {
            try
            {
                var table = _driver.FindElement(By.XPath("//*[@id=\"app\"]/div/main/article/table/tbody"), _config.Timeout);
                var maxTr = table.FindElements(By.XPath("//tbody//tr")).Count;
                var loop = true;
                while (loop)
                {
                    table = _driver.FindElement(By.XPath("//*[@id=\"app\"]/div/main/article/table/tbody"), _config.Timeout);

                    maxTr = table.FindElements(By.XPath("//tbody//tr")).Count;
                    for (int j = 1; j <= maxTr; j++)
                    {
                        Console.WriteLine(
                            table.FindElement(By.XPath($"//tr[{j}]/td[1]")).Text + "\t" +
                            table.FindElement(By.XPath($"//tr[{j}]/td[2]")).Text + "\t" +
                            table.FindElement(By.XPath($"//tr[{j}]/td[3]")).Text + "\t" +
                            table.FindElement(By.XPath($"//tr[{j}]/td[4]")).Text
                            );
                    }

                    if (!_driver.ExistElement(By.XPath("//button[contains(text(),'Proxima')]"), _config.Timeout))
                        loop = false;
                    ClickNextPage();

                }
            }
            catch (Exception)
            {

            }
        }

        /*
         * Clique com XPath  
         */
        public void XPathClick(string xpath, bool generateThrow = true)
        {
            try
            {
                _driver.FindElement(By.XPath(xpath), _config.Timeout).Click();
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine($"Elemento nao encontrado: ${xpath}");
                if (generateThrow)
                    throw new TimeoutException($"Elemento nao encontrado: ${xpath}");
            }
            _driver.WaitMS(_config.Timeout);
        }

    }
}
