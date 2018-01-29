using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using RevolutionaryStuff.JBT.Streams;

namespace StreamMuxerExample
{
    public class TestObj
    {
        public static readonly XmlSerializer Serializer = new XmlSerializer(typeof(TestObj));
        private static readonly Random R = new Random(Environment.TickCount);

        public int A;
        public string B;
        public byte[] RandomBinaryData;

        public TestObj(){}
        public TestObj(int a, string b)
        {
            this.A = a;
            this.B = b;
            this.RandomBinaryData = new byte[R.Next(256)];
            R.NextBytes(this.RandomBinaryData);            
        }

        public override string ToString()
        {
            return string.Format("TestObj: A={0} B={1} RandomBinaryDataLen={2}", this.A, this.B, this.RandomBinaryData.Length);
        }
    }

    /// <summary>
    /// This is an example program to show some of the benefits of the Stream Muxer
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            //The format is #Objects, SizeObj0, Obj0... SizeObjN, ObjN
            //This is a combo of binary and xml data
            MemoryStream st = new MemoryStream();

            //Create a stream the old fashioned way
            Console.WriteLine("Creating Stream==========");
            Debug.WriteLine("Creating Stream==========");
            BinaryWriter w = new BinaryWriter(st);
            for (int z = 0; z < 31; ++z)
            {
                st.Position = 0;
                w.Write(z+1);
                long objSizeOffset = st.Length;
                st.SetLength(st.Length + 8);
                st.Seek(0, SeekOrigin.End);
                TestObj o = new TestObj(z, string.Format("Test Object #{0}", z));
                Console.WriteLine(o);
                Debug.WriteLine(o);
                TestObj.Serializer.Serialize(st, o);
                long size = st.Length - objSizeOffset - 8;
                st.Position = objSizeOffset;
                w.Write(size);
                st.Flush();
            }

            //So you can examine the stream contents in the memory window
            byte[] buf = st.ToArray();

            //now let's de-serialize every other object with the muxer
            Console.WriteLine("Reading Stream==========");
            Debug.WriteLine("Reading Stream==========");
            st.Position = 0;
            using (StreamMuxer muxer = new StreamMuxer(st))
            {
                using (Stream binaryPartsStream = muxer.Create(true, false))
                {
                    BinaryReader r = new BinaryReader(binaryPartsStream);
                    int objCnt = r.ReadInt32();
                    long basePos = 4;
                    for (int z = 0; z < objCnt; ++z)
                    {
                        long size = r.ReadInt64();
                        basePos += 8;
                        //jump the binary stream past the xml data
                        //note that i am seeking pas the data, not setting the position.
                        //this should show the independance of the 2 streams!
                        binaryPartsStream.Seek(size, SeekOrigin.Current);
                        //if every other...
                        if (z % 2 == 0)
                        {
                            //create a new readonly stream positioned at the current spot
                            //But every now and then, artificially expand the new stream to a larger size
                            //to force an exeption.  in doing so, the outer stream of course retains
                            //it's state, an is unaffected!
                            long s = size + ((z % 8 == 2) ? 40 : 0);
                            using (Stream xmlPartStream = muxer.Create(true, false, basePos, s))
                            {
                                try
                                {
                                    Debug.WriteLine(string.Format("XmlPartStream: BEFORE read of obj {0}. xmlSize={1} size={2} pos={3}", z, size, xmlPartStream.Length, xmlPartStream.Position));
                                    TestObj testObj = (TestObj)TestObj.Serializer.Deserialize(xmlPartStream);
                                    Console.WriteLine(testObj);
                                    Debug.WriteLine(testObj);
                                    Debug.WriteLine(string.Format("XmlPartStream: AFTER read of obj {0}. xmlSize={1} size={2} pos={3}", z, size, xmlPartStream.Length, xmlPartStream.Position));
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine("Object creation failed since stream size was expanded past bounds and serializer could not recognize binary data after the xml data!");
                                }
                            }
                        }
                        basePos += size;
                    }
                }
            }   
        }
    }
}
