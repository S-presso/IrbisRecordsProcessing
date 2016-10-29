/* DataProcessingEventHandler.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

#endregion

namespace AM.IO.Evented
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="ea"></param>
    public delegate void DataProcessingEventHandler ( object sender,
                                                      DataProcessingEventArgs ea
        );
}
