﻿using System;
using System.ComponentModel.DataAnnotations;

namespace SpojDebug.Core.Entities
{
    public abstract class BaseEntity<TKey> where TKey : struct 
    {
        [Key]
        public TKey Id { get; set; }
        //public int? CreatedBy { get; set; }
        public DateTimeOffset? CreatedTime { get; set; }
        //public int? LastUpdatedBy { get; set; }
        //public DateTimeOffset? LastUpdatedTime { get; set; }
        //public int? DeletedBy { get; set; }
        //public DateTimeOffset? DeletedTime { get; set; }
    }
}
