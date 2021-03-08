using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace TheatreCMS.Models
{
    public class News
    {
        [Key]
        public int NewsId { get; set; }                 //News primary key
        
        [AllowHtml]
        public string Headline { get; set; }            //News headline

        [AllowHtml]
        public string Content { get; set; }             //news content
        public DateTime? CreateDate { get; set; }       //create date of news article (nullable)
        public DateTime? LastSaveDate { get; set; }     //last date of edit of news article (nullable)
        public DateTime? PublishDate { get; set; }      //publish date of news article (nullable)
        public DateTime? EmailDate { get; set; }        //sent email date to subsribers (nullable)
        public bool Hidden { get; set; }                //hidden from non-admin users
    }
}