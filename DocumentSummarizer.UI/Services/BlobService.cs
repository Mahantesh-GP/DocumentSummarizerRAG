public class BlobService
{
    private readonly IConfiguration _config;

    public BlobService(IConfiguration config)
    {
        _config = config;
    }

    public string GetDownloadUrl(string blobPath)
    {
        string containerUrl = _config["BlobStorage:ContainerUrl"];
        return $"{containerUrl}/{blobPath}";
    }

    public string GetFileName(string blobPath)
    {
        return Path.GetFileName(blobPath);
    }
}