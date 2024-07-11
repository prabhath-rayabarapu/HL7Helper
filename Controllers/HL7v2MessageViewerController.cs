using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Web;

namespace HL7Helper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HL7v2MessageViewerController : ControllerBase
    {
        public class Field
        {
            public int Position { get; set; }
            public string Value { get; set; }
        }

        public class Segment
        {
            public string SegmentName { get; set; }
            public List<Field> Fields { get; set; } = new List<Field>();
        }

        [HttpPost]
        public IActionResult ParseHL7Message([FromBody] string hl7Message)
        {
            hl7Message = HttpUtility.UrlDecode(hl7Message);
            const string fieldSeparator = "|";

            var segments = new List<Segment>();

            var lines = hl7Message.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var fields = line.Split(fieldSeparator);
                var segment = new Segment
                {
                    SegmentName = fields[0]
                };

                for (int i = 1; i < fields.Length; i++)
                {
                    var field = fields[i];
                    segment.Fields.Add(new Field
                    {
                        Position = i,
                        Value = field
                    });
                }

                segments.Add(segment);
            }

            return Ok(segments);
        }
    }
}
