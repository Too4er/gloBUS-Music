using gloBUS_Music.Data;
using System.Collections.Generic;
using System.Linq;
using gloBUS_Music.MVVM.Model;

namespace gloBUS_Music.MVVM.Services;
public class TrackService
{
    private readonly gloBUS_MusicDbContext _context;

    public TrackService(gloBUS_MusicDbContext context)
    {
        _context = context;
    }

    public List<Track> GetAllTracks()
    {
        return _context.Tracks.ToList();
    }

    public void AddTrack(Track track)
    {
        _context.Tracks.Add(track);
        _context.SaveChanges();
    }

    public void DeleteTrack(int id)
    {
        var track = _context.Tracks.FirstOrDefault(t => t.Id == id);
        if (track != null)
        {
            _context.Tracks.Remove(track);
            _context.SaveChanges();
        }
    }
}