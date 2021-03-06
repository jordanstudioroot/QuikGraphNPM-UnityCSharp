using System;


namespace QuikGraph
{
    /// <summary>
    /// Represents an object that is able to be tagged.
    /// </summary>
    /// <typeparam name="TTag">Tag type.</typeparam>
    public interface ITagged<TTag>
    {
        /// <summary>
        /// Fired when the tag is changed.
        /// </summary>
        event EventHandler TagChanged;

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        
        TTag Tag { get; set; }
    }
}
