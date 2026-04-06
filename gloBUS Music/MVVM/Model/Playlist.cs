using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace gloBUS_Music.MVVM.Model
{
    public class Playlist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Track> Tracks { get; set; }

        public Playlist()
        {
            Tracks = new List<Track>();
        }
    }
}
