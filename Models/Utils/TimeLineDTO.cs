using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Site.Models {
    public class TimeLineDTO {
        public TimeLineHeader timeline { get; set; }

        public TimeLineDTO() {
            timeline = new TimeLineHeader();
        }

        public class TimeLineHeader {
            public string headline  { get; set; }   
            public string type { get; set; }   
            public string text { get; set; }   
            public string startDate { get; set; }   
            public List<TimeLineDTO.Item> date { get; set; }   


            public TimeLineHeader()  {
                date = new List<TimeLineDTO.Item>();
            }
        }

        public class Item {
            public string startDate { get; set; }
            public string headline { get; set; }
            public string text { get; set; }
        }

    }



}
