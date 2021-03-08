using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheatreCMS.Models;

namespace TheatreCMS.ViewModels
{
    public class PhotoDependenciesVm
    {
        public List<ProductionPhotos> ProductionPhotos { get; set; } = new List<ProductionPhotos>();
        public List<CastMember> CastMembers { get; set; } = new List<CastMember>();
        public List<Sponsor> Sponsors { get; set; } = new List<Sponsor>();
        public bool HasDependencies { get; set; }
        public bool ValidId { get; set; }
    }
}