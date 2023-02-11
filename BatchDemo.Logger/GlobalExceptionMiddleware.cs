using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Core;

namespace BatchDemo.Logger
{
    public static class GlobalExceptionMiddleware
    {
        public static void UseGlobalExceptionMiddleware(this IApplicationBuilder app,Serilog.Core.Logger logger)
        {
            app.UseMiddleware<ExceptionMiddleware>(logger);
        }
    }
}
