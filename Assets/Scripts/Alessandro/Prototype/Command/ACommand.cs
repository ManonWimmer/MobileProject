using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandContext
{
    public Vector2 TileAimed{ get; set;  }
    public int SenderID { get; set; }
    public int ReceiverID { get; set; }
}

public abstract class ACommand<T>
{
    protected ShipManager _receiver;
    protected ShipManager _sender;
    protected scriptablePower _data;

    public void Init(ShipManager sender, ShipManager receiver, scriptablePower _data)
    {
        this._sender = sender;
        this._receiver = receiver;
        this._data = _data;
    }

    public abstract IEnumerator Execute(CommandContext args);

    public abstract IEnumerator Preview(CommandContext args);
}
