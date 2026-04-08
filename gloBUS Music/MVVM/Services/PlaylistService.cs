using gloBUS_Music.Data;
using gloBUS_Music.MVVM.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace gloBUS_Music.MVVM.Services
{
    public class PlaylistService
    {
        private readonly gloBUS_MusicDbContext _context;

        public PlaylistService(gloBUS_MusicDbContext context)
        {
            _context = context;
        }

        public List<Playlist> GetAllPlaylists()
        {
            return _context.Playlists
                .Include(p => p.Tracks)
                .OrderBy(p => p.Name)
                .ToList();
        }

        public Playlist GetPlaylistById(int id)
        {
            return _context.Playlists
                .Include(p => p.Tracks)
                .FirstOrDefault(p => p.Id == id);
        }

        public Playlist CreatePlaylist(string name)
        {
            var playlist = new Playlist
            {
                Name = name
            };

            _context.Playlists.Add(playlist);
            _context.SaveChanges();

            return playlist;
        }

        public bool DeletePlaylist(int playlistId)
        {
            var playlist = _context.Playlists
                .Include(p => p.Tracks)
                .FirstOrDefault(p => p.Id == playlistId);

            if (playlist == null)
                return false;

            _context.Playlists.Remove(playlist);
            _context.SaveChanges();

            return true;
        }

        public bool AddTrackToPlaylist(int playlistId, int trackId)
        {
            var playlist = _context.Playlists
                .Include(p => p.Tracks)
                .FirstOrDefault(p => p.Id == playlistId);

            var track = _context.Tracks.FirstOrDefault(t => t.Id == trackId);

            if (playlist == null || track == null)
                return false;

            if (playlist.Tracks.Any(t => t.Id == trackId))
                return false;

            playlist.Tracks.Add(track);
            _context.SaveChanges();

            return true;
        }

        public bool RemoveTrackFromPlaylist(int playlistId, int trackId)
        {
            var playlist = _context.Playlists
                .Include(p => p.Tracks)
                .FirstOrDefault(p => p.Id == playlistId);

            if (playlist == null)
                return false;

            var track = playlist.Tracks.FirstOrDefault(t => t.Id == trackId);

            if (track == null)
                return false;

            playlist.Tracks.Remove(track);
            _context.SaveChanges();

            return true;
        }
    }
}