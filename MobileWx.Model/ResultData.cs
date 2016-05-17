using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileWx.Model
{

    public class ResultData
    {
        public ResultData()
        {
            this.UpdateTime = DateTime.Now;
            this.Status = -1;
        }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

        [JsonProperty("updatetime")]
        public DateTime UpdateTime { get; set; }
    }
}
