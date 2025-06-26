namespace AcervoEducacional.Application.DTOs.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> SuccessResult(T data, string message = "Operação realizada com sucesso")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> ErrorResult(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }

    public static ApiResponse<T> ErrorResult(List<string> errors)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = "Erro de validação",
            Errors = errors
        };
    }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }

    public static PagedResult<T> Create(List<T> items, int totalItems, int page, int pageSize)
    {
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        
        return new PagedResult<T>
        {
            Items = items,
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            HasNextPage = page < totalPages,
            HasPreviousPage = page > 1
        };
    }
}

public class SelectListItem
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool Selected { get; set; } = false;
}

public class KeyValueDto
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class FileUploadResult
{
    public bool Success { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}

public class BulkOperationResult
{
    public int TotalItems { get; set; }
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public List<string> Errors { get; set; } = new();
    public bool HasErrors => ErrorCount > 0;
    public bool IsPartialSuccess => SuccessCount > 0 && ErrorCount > 0;
    public bool IsCompleteSuccess => SuccessCount > 0 && ErrorCount == 0;
}

