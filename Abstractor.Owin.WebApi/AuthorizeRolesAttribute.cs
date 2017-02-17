using System.Text;
using System.Web.Http;

namespace Abstractor.Owin.WebApi
{
    /// <summary>
    ///     Extends the authorize attribute receiving an array of roles.
    /// </summary>
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        /// <summary>
        ///     Extends the authorize attribute receiving an array of roles.
        /// </summary>
        /// <param name="roles">Array of roles.</param>
        public AuthorizeRolesAttribute(params string[] roles)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < roles.Length; i++)
            {
                if (string.IsNullOrEmpty(roles[i])) continue;
                var separator = i != roles.Length ? "," : string.Empty;
                sb.Append(roles[i] + separator);
            }

            Roles = sb.Length > 0
                ? sb.Remove(sb.Length - 1, 1).ToString()
                : string.Empty;
        }
    }
}