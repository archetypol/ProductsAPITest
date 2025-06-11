using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ProductsApi.Common.DbHelpers
{
    public static class SqlExceptionHelper
    {
        // little helper to see if an insert violates a constraint
        // wild that this isnt built in
        public static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                // 2627 = Violation of PRIMARY KEY or UNIQUE constraint
                // 2601 = Cannot insert duplicate key row with unique index
                return sqlEx.Number == 2627 || sqlEx.Number == 2601;
            }

            return false;
        }
    }
}
