﻿using MAAV.WebAPI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MAAV.WebAPI.Serializers
{
    public class SnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public static SnakeCaseNamingPolicy Instance { get; } = new SnakeCaseNamingPolicy();

        public override string ConvertName(string name)
        {
            // Conversion to other naming conventaion goes here. Like SnakeCase, KebabCase etc.
            return name.ToSnakeCase();
        }
    }
}
