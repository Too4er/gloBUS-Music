using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gloBUS_Music.MVVM.Model
{
    public class Track
    {
        public int Id { get; set; } // ID
        public string Title { get; set; } // Название
        public string Artist { get; set; } // Исполнитель
        public int Duration { get; set; } // Длительность
        public string Link { get; set; } // Ссылки

        public List<Playlist> Playlists { get; set; }

        public Track()
        {
            Playlists = new List<Playlist>();
        }

    }
}
