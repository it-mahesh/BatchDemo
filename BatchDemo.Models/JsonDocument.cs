﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatchDemo.Models
{
    public class JsonDocument
    {
        public Guid? BatchId { get; set; }
        public string Document { get; set; } = string.Empty;
    }

}
