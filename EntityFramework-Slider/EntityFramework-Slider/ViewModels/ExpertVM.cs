﻿using EntityFramework_Slider.Models;

namespace EntityFramework_Slider.ViewModels
{
    public class ExpertVM
    {
        public IEnumerable<Experts> Experts { get; set; }

        public ExpertsHeader ExpertsHeader { get; set; }

    }
}

