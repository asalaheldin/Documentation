﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Documentation.Ground
{
    public static class Extensions
    {
        public static string GetUserId(this IPrincipal principal)
        {
            var claim = ((ClaimsIdentity)principal.Identity).FindFirst("UserId");
            return claim == null ? null : claim.Value;
        }
        public static string GetUserName(this IPrincipal principal)
        {
            var claim = ((ClaimsIdentity)principal.Identity).FindFirst("UserName");
            return claim == null ? null : claim.Value;
        }
        public static string GetFirstName(this IPrincipal principal)
        {
            var claim = ((ClaimsIdentity)principal.Identity).FindFirst("FirstName");
            return claim == null ? null : claim.Value;
        }
        public static string GetLastName(this IPrincipal principal)
        {
            var claim = ((ClaimsIdentity)principal.Identity).FindFirst("LastName");
            return claim == null ? null : claim.Value;
        }
        public static string GetFullName(this IPrincipal principal)
        {
            var claim = ((ClaimsIdentity)principal.Identity).FindFirst("FullName");
            return claim == null ? null : claim.Value;
        }
        public static IQueryable<T> Where<T>(this IQueryable<T> source, string columnName, bool keyword)
        {
            var arg = Expression.Parameter(typeof(T), "p");
            var body = Expression.Call(
                Expression.Property(arg, columnName), "Equals", null,
                Expression.Constant(keyword));
            var predicate = Expression.Lambda<Func<T, bool>>(body, arg);
            return source.Where(predicate);
        }
    }
}
