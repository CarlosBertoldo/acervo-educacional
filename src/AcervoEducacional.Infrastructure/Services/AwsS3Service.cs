using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AcervoEducacional.Application.Interfaces;

namespace AcervoEducacional.Infrastructure.Services;

public class AwsS3Service : IAwsS3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AwsS3Service> _logger;

    public AwsS3Service(IAmazonS3 s3Client, IConfiguration configuration, ILogger<AwsS3Service> logger)
    {
        _s3Client = s3Client;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> UploadFileAsync(IFormFile file, string bucketName, string keyName)
    {
        try
        {
            using var stream = file.OpenReadStream();
            
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = keyName,
                InputStream = stream,
                ContentType = file.ContentType,
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
                Metadata =
                {
                    ["original-name"] = file.FileName,
                    ["upload-date"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                }
            };

            var response = await _s3Client.PutObjectAsync(request);
            
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                _logger.LogInformation("Arquivo {FileName} enviado com sucesso para S3: {Key}", file.FileName, keyName);
                return keyName;
            }
            
            throw new Exception($"Falha no upload para S3. Status: {response.HttpStatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer upload do arquivo {FileName} para S3", file.FileName);
            throw;
        }
    }

    public async Task<string> GetPresignedUrlAsync(string bucketName, string keyName, TimeSpan expiration)
    {
        try
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = keyName,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.Add(expiration)
            };

            var url = await _s3Client.GetPreSignedURLAsync(request);
            
            _logger.LogDebug("URL pré-assinada gerada para {Key}, expira em {Expiration}", keyName, expiration);
            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar URL pré-assinada para {Key}", keyName);
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(string bucketName, string keyName)
    {
        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = keyName
            };

            var response = await _s3Client.DeleteObjectAsync(request);
            
            var success = response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
            
            if (success)
            {
                _logger.LogInformation("Arquivo {Key} excluído com sucesso do S3", keyName);
            }
            else
            {
                _logger.LogWarning("Falha ao excluir arquivo {Key} do S3. Status: {Status}", keyName, response.HttpStatusCode);
            }
            
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir arquivo {Key} do S3", keyName);
            return false;
        }
    }

    public async Task<bool> FileExistsAsync(string bucketName, string keyName)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = bucketName,
                Key = keyName
            };

            await _s3Client.GetObjectMetadataAsync(request);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência do arquivo {Key} no S3", keyName);
            throw;
        }
    }

    public async Task<long> GetFileSizeAsync(string bucketName, string keyName)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = bucketName,
                Key = keyName
            };

            var response = await _s3Client.GetObjectMetadataAsync(request);
            return response.ContentLength;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter tamanho do arquivo {Key} no S3", keyName);
            throw;
        }
    }

    public async Task<string> CopyFileAsync(string sourceBucket, string sourceKey, string destBucket, string destKey)
    {
        try
        {
            var request = new CopyObjectRequest
            {
                SourceBucket = sourceBucket,
                SourceKey = sourceKey,
                DestinationBucket = destBucket,
                DestinationKey = destKey,
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
            };

            var response = await _s3Client.CopyObjectAsync(request);
            
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                _logger.LogInformation("Arquivo copiado com sucesso de {SourceKey} para {DestKey}", sourceKey, destKey);
                return destKey;
            }
            
            throw new Exception($"Falha na cópia do arquivo. Status: {response.HttpStatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao copiar arquivo de {SourceKey} para {DestKey}", sourceKey, destKey);
            throw;
        }
    }
}

