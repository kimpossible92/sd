﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteamNetworking.Messages
{
    class MessageNetworkObjectList
    {
        // Dynamic size, stores byte representations of network object messages
        public LinkedList<byte[]> messages = new LinkedList<byte[]>();    // ? bytes

        public int GetLength()
        {
            // Message count
            int length = 4;

            foreach (byte[] b in messages)
            {
                length += 4 + b.Length;
            }

            return length;
        }

        public byte[] ToBytes()
        {
            ArrayList data = new ArrayList();

            // Dynamic size
            data.AddRange(BitConverter.GetBytes(messages.Count));

            foreach (byte[] b in messages)
            {
                data.AddRange(BitConverter.GetBytes(b.Length));
                data.AddRange(b);
            }

            return (byte[])data.ToArray(typeof(byte));
        }

        public static MessageNetworkObjectList FromBytes(byte[] data, int startIndex)
        {
            MessageNetworkObjectList messageNetworkObjectList = new MessageNetworkObjectList();

            // Get messages length
            int messagesCount = BitConverter.ToInt32(data, startIndex);
            int index = startIndex + 4;

            // Read and assign all the dynamically sized network object messages
            for (int i = 0; i < messagesCount; i++)
            {
                int bLength = BitConverter.ToInt32(data, index);
                index += 4;

                byte[] b = new byte[bLength];
                Array.Copy(data, index, b, 0, bLength);
                index += bLength;

                messageNetworkObjectList.messages.AddLast(b);
            }

            return messageNetworkObjectList;
        }
    }
    public class MessageNetworkObject
    {
        // Fixed size
        public float time;                                          // 4 bytes
        public bool hasParent;                                      // 1 byte
        public int resourceID;                                      // 4 bytes
        public int instanceID;                                      // 4 bytes
        public int rootInstanceID;                                  // 4 bytes
        public int parentInstanceID;                                // 4 bytes
        public Vector3 localPosition;                               // 12 bytes
        public Quaternion localRotation;                            // 16 bytes
        public Vector3 localScale;                                  // 12 bytes

        // Dynamic size
        public string name;                                         // ? bytes

        public MessageNetworkObject()
        {
            // Empty constructor
        }

        public MessageNetworkObject(NetworkObject networkObject)
        {
            if (networkObject.transform.parent != null)
            {
                parentInstanceID = networkObject.transform.parent.GetInstanceID();
                hasParent = true;
            }
            else
            {
                parentInstanceID = 0;
                hasParent = false;
            }

            // Set and save the last update time of the network object
            time = networkObject.lastUpdate = Time.unscaledTime;
            name = networkObject.name;
            resourceID = networkObject.resourceID;
            instanceID = networkObject.transform.GetInstanceID();
            rootInstanceID = networkObject.root.transform.GetInstanceID();
            localPosition = networkObject.transform.localPosition;
            localRotation = networkObject.transform.localRotation;
            localScale = networkObject.transform.localScale;
        }

        public byte[] ToBytes()
        {
            ArrayList data = new ArrayList();

            // Fixed size
            data.AddRange(BitConverter.GetBytes(time));
            data.AddRange(BitConverter.GetBytes(hasParent));
            data.AddRange(BitConverter.GetBytes(resourceID));
            data.AddRange(BitConverter.GetBytes(instanceID));
            data.AddRange(BitConverter.GetBytes(rootInstanceID));
            data.AddRange(BitConverter.GetBytes(parentInstanceID));
            data.AddRange(BitConverter.GetBytes(localPosition.x));
            data.AddRange(BitConverter.GetBytes(localPosition.y));
            data.AddRange(BitConverter.GetBytes(localPosition.z));
            data.AddRange(BitConverter.GetBytes(localRotation.x));
            data.AddRange(BitConverter.GetBytes(localRotation.y));
            data.AddRange(BitConverter.GetBytes(localRotation.z));
            data.AddRange(BitConverter.GetBytes(localRotation.w));
            data.AddRange(BitConverter.GetBytes(localScale.x));
            data.AddRange(BitConverter.GetBytes(localScale.y));
            data.AddRange(BitConverter.GetBytes(localScale.z));

            // Dynamic size
            byte[] nameData = System.Text.Encoding.UTF8.GetBytes(name);
            data.AddRange(BitConverter.GetBytes(nameData.Length));
            data.AddRange(nameData);

            return (byte[])data.ToArray(typeof(byte));
        }

        public static MessageNetworkObject FromBytes(byte[] data, int startIndex)
        {
            MessageNetworkObject messageNetworkObject = new MessageNetworkObject();

            // Fixed size
            messageNetworkObject.time = BitConverter.ToSingle(data, startIndex);
            messageNetworkObject.hasParent = BitConverter.ToBoolean(data, startIndex + 4);
            messageNetworkObject.resourceID = BitConverter.ToInt32(data, startIndex + 5);
            messageNetworkObject.instanceID = BitConverter.ToInt32(data, startIndex + 9);
            messageNetworkObject.rootInstanceID = BitConverter.ToInt32(data, startIndex + 13);
            messageNetworkObject.parentInstanceID = BitConverter.ToInt32(data, startIndex + 17);
            messageNetworkObject.localPosition.x = BitConverter.ToSingle(data, startIndex + 21);
            messageNetworkObject.localPosition.y = BitConverter.ToSingle(data, startIndex + 25);
            messageNetworkObject.localPosition.z = BitConverter.ToSingle(data, startIndex + 29);
            messageNetworkObject.localRotation.x = BitConverter.ToSingle(data, startIndex + 33);
            messageNetworkObject.localRotation.y = BitConverter.ToSingle(data, startIndex + 37);
            messageNetworkObject.localRotation.z = BitConverter.ToSingle(data, startIndex + 41);
            messageNetworkObject.localRotation.w = BitConverter.ToSingle(data, startIndex + 45);
            messageNetworkObject.localScale.x = BitConverter.ToSingle(data, startIndex + 49);
            messageNetworkObject.localScale.y = BitConverter.ToSingle(data, startIndex + 53);
            messageNetworkObject.localScale.z = BitConverter.ToSingle(data, startIndex + 57);

            // Dynamic size
            int nameDataLength = BitConverter.ToInt32(data, startIndex + 61);
            messageNetworkObject.name = System.Text.Encoding.UTF8.GetString(data, startIndex + 65, nameDataLength);

            return messageNetworkObject;
        }
    }
}