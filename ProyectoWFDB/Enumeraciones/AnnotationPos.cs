
namespace ProyectoWFDB.Enumeraciones
{
    /// <summary>
    /// These may be used by applications which plot
    /// signals and annotations to determine where to print annotation mnemonics.
    /// </summary>
    public enum AnnotationPos
    {
        /// <summary>
        /// Undefined annotation types 
        /// </summary>
        APUndef = 0,

        /// <summary>
        /// Standard position 
        /// </summary>
        APStd = 1,

        /// <summary>
        /// A level above <see cref="APStd"/>
        /// </summary>
        APHigh = 2,

        /// <summary>
        /// A level below <see cref="APStd"/>
        /// </summary>
        APLow = 3,

        /// <summary>
        /// Attached to the signal specified by `chan' 
        /// </summary>
        APAtt = 4,

        /// <summary>
        /// A level above <see cref="APAtt"/>
        /// </summary>
        APAHigh = 5,

        /// <summary>
        /// A level below <see cref="APAtt"/>
        /// </summary>
        APALow = 6,
    }
}
