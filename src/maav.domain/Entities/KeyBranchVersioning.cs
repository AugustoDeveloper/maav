﻿using System;

namespace MAAV.Domain.Entities
{
    public class KeyBranchVersioning
    {
        public string KeyBranchName { get; set; }
        public SemanticVersion CurrentVersion { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string Commit { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
