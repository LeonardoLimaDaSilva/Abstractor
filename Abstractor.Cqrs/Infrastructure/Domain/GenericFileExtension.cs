namespace Abstractor.Cqrs.Infrastructure.Domain
{
    /// <summary>
    ///     Extension for the <see cref="GenericFile" /> class.
    /// </summary>
    public static class GenericFileExtension
    {
        /// <summary>
        ///     Verifies if the instance is valid.
        /// </summary>
        /// <param name="genericFile">Instance to be verified.</param>
        /// <returns></returns>
        public static bool IsValid(this GenericFile genericFile)
        {
            return (genericFile != null) &&
                   string.IsNullOrEmpty(genericFile.FileName) &&
                   (genericFile.Stream != null) &&
                   (genericFile.Stream.Length > 0);
        }
    }
}