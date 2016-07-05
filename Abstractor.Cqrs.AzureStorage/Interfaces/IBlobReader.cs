using System;
using Abstractor.Cqrs.AzureStorage.Blob;

namespace Abstractor.Cqrs.AzureStorage.Interfaces
{
    public interface IBlobReader<out TEntity> where TEntity : AzureBlob
    {
        bool Exists(string fileName);
        TEntity Get(string fileName);
        Uri GetVirtualPath(string fileName);
    }
}