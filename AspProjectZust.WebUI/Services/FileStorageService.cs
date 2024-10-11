namespace AspProjectZust.WebUI.Helpers
{
    public class FileStorageService
    {
        private readonly IWebHostEnvironment _webHost;

        public FileStorageService(IWebHostEnvironment webHost)
        {
            _webHost = webHost;
        }

        public async Task<string> SaveFile(IFormFile file)
        {
            var saveImg = Path.Combine(_webHost.WebRootPath, "assets/images/user", file.FileName);
            using (var img = new FileStream(saveImg, FileMode.OpenOrCreate))
            {
                await file.CopyToAsync(img);
            }
            return file.FileName.ToString();
        }
    }
}
