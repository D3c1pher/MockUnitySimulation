using System.Collections;
using System.Collections.Generic;
using TestTask.NonEditable;
using UnityEngine;

public class PacketLatencyMock : MonoBehaviour
{
    public List<(float sendTimestamp, byte[] message)> MessagesQueue { get; private set; } = new List<(float, byte[])>();
    [SerializeField] private IPacketSender _packetSender;

    [SerializeField] private AnimationCurve _latencyValueCurve;

    public void Initialize(IPacketSender packetSender)
    {
        _packetSender = packetSender;
    }

    public void EnqueueMessage(byte[] message)
    {
        var randomValue = Random.Range(0f, 100f);

        var latencyValueMS = _latencyValueCurve.Evaluate(randomValue)/100f;

        var sendTimestamp = Time.time + latencyValueMS;
        MessagesQueue.Add((sendTimestamp, message));
    }

    public void FixedUpdate()
    {
        if(_packetSender == null)
            return;

        for (int i = MessagesQueue.Count - 1; i >= 0; i--)
        {
            var (sendTimestamp, message) = MessagesQueue[i];
            if (Time.time >= sendTimestamp)
            {
                _packetSender.Send(message);
                MessagesQueue.RemoveAt(i);
            }
        }
    }

}
