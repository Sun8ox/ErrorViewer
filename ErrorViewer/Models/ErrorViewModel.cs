namespace ErrorViewer.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}

public class SimpleError
{
    public string? error { get; set; } = "Something went wrong";
    public int? id { get; set; }

}