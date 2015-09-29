using System.Data.Entity;

namespace Abstractor.Cqrs.SimpleInjector.EntityFramework.ModelCreation
{
    /// <summary>
    /// Especifica o modelo de criação do esquema do banco de dados.
    /// </summary>
    public interface ICreateDbModel
    {
        /// <summary>
        /// Cria o esquema do banco de dados. Este método deve ser executado através do <see cref="DbContext.OnModelCreating"/>.
        /// </summary>
        /// <param name="modelBuilder">O model builder.</param>
        void Create(DbModelBuilder modelBuilder);
    }
}
