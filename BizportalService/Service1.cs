using System.ServiceProcess;

namespace BizportalService
{
    /// <summary>
    /// Simple service to start and stop a batch process.
    /// 
    /// Looks for an environment variable: BIZPORTAL_SETTINGS on start up,
    /// which should be the fully qualified path to the settings file.
    ///
    /// If it is not found the default path is:
    ///
    /// d:\bizportalReskin\api\StartReskinApi.bat
    ///
    /// </summary>
    public partial class Service1 : ServiceBase
    {
        private Kernel _kernel;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _kernel = new Kernel();
        }

        protected override void OnStop()
        {
            _kernel?.Dispose();
        }
    }
}
