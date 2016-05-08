#region Header
// **************************************\
//     _  _   _   __  ___  _   _   ___   |
//    |# |#  |#  |## |### |#  |#  |###   |
//    |# |#  |# |#    |#  |#  |# |#  |#  |
//    |# |#  |#  |#   |#  |#  |# |#  |#  |
//   _|# |#__|#  _|#  |#  |#__|# |#__|#  |
//  |##   |##   |##   |#   |##    |###   |
//        [http://www.playuo.org]        |
// **************************************/
//  [2014] ClientVersion.cs
// ************************************/
#endregion

#region References
using System;
using System.Collections;
using System.Text;
#endregion

namespace Server
{
	public enum ClientType
	{
		Regular,
		UOTD,
		God,
		KR,
        EC
	}

	public class ClientVersion : IComparable, IComparer
	{
		private readonly int m_Major;
		private readonly int m_Minor;
		private readonly int m_Revision;
		private readonly int m_Patch;
		private readonly ClientType m_Type;
		private readonly string m_SourceString;

		public int Major { get { return m_Major; } }

		public int Minor { get { return m_Minor; } }

		public int Revision { get { return m_Revision; } }

		public int Patch { get { return m_Patch; } }

		public ClientType Type { get { return m_Type; } }

		public string SourceString { get { return m_SourceString; } }

		public ClientVersion(int maj, int min, int rev, int pat)
			: this(maj, min, rev, pat, ClientType.Regular)
		{ }

		public ClientVersion(int maj, int min, int rev, int pat, ClientType type)
		{
			m_Major = maj;
			m_Minor = min;
			m_Revision = rev;
			m_Patch = pat;
			m_Type = type;

			m_SourceString = _ToStringImpl();
		}

		public static bool operator ==(ClientVersion l, ClientVersion r)
		{
			return (Compare(l, r) == 0);
		}

		public static bool operator !=(ClientVersion l, ClientVersion r)
		{
			return (Compare(l, r) != 0);
		}

		public static bool operator >=(ClientVersion l, ClientVersion r)
		{
			return (Compare(l, r) >= 0);
		}

		public static bool operator >(ClientVersion l, ClientVersion r)
		{
			return (Compare(l, r) > 0);
		}

		public static bool operator <=(ClientVersion l, ClientVersion r)
		{
			return (Compare(l, r) <= 0);
		}

		public static bool operator <(ClientVersion l, ClientVersion r)
		{
			return (Compare(l, r) < 0);
		}

		public override int GetHashCode()
		{
			return m_Major ^ m_Minor ^ m_Revision ^ m_Patch;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}

			var v = obj as ClientVersion;

			if (v == null)
			{
				return false;
			}

			return m_Major == v.m_Major && m_Minor == v.m_Minor && m_Revision == v.m_Revision && m_Patch == v.m_Patch;
		}

		private string _ToStringImpl()
		{
			var builder = new StringBuilder(16);

			builder.Append(m_Major);
			builder.Append('.');
			builder.Append(m_Minor);
			builder.Append('.');
			builder.Append(m_Revision);

			if (m_Major <= 5 && m_Minor <= 0 && m_Revision <= 6) //Anything before 5.0.7
			{
				if (m_Patch > 0)
				{
					builder.Append((char)('a' + (m_Patch - 1)));
				}
			}
			else
			{
				builder.Append('.');
				builder.Append(m_Patch);
			}

			if (m_Type != ClientType.Regular)
			{
				builder.Append(' ');
				builder.Append(m_Type.ToString());
			}

			return builder.ToString();
		}

		public override string ToString()
		{
			return _ToStringImpl();
		}

		public ClientVersion(string fmt)
		{
			m_SourceString = fmt;

			try
			{
				fmt = fmt.ToLower();

				int br1 = fmt.IndexOf('.');
				int br2 = fmt.IndexOf('.', br1 + 1);

				int br3 = br2 + 1;
				while (br3 < fmt.Length && Char.IsDigit(fmt, br3))
				{
					br3++;
				}

				m_Major = Utility.ToInt32(fmt.Substring(0, br1));
				m_Minor = Utility.ToInt32(fmt.Substring(br1 + 1, br2 - br1 - 1));
				m_Revision = Utility.ToInt32(fmt.Substring(br2 + 1, br3 - br2 - 1));

				if (br3 < fmt.Length)
				{
					if (m_Major <= 5 && m_Minor <= 0 && m_Revision <= 6) //Anything before 5.0.7
					{
						if (!Char.IsWhiteSpace(fmt, br3))
						{
							m_Patch = (fmt[br3] - 'a') + 1;
						}
					}
					else
					{
						m_Patch = Utility.ToInt32(fmt.Substring(br3 + 1, fmt.Length - br3 - 1));
					}
				}

                #region Enhance Client
                if (fmt.IndexOf("ec") >= 0)
                {
                    m_Type = ClientType.EC;
                }
                else if (fmt.IndexOf("kr") >= 0)
                {
                    m_Type = ClientType.KR;
                }
                else if (fmt.IndexOf("god") >= 0 || fmt.IndexOf("gq") >= 0)
                {
                    m_Type = ClientType.God;
                }
                else if (fmt.IndexOf("third dawn") >= 0 || fmt.IndexOf("uo:td") >= 0 || fmt.IndexOf("uotd") >= 0 ||
                         fmt.IndexOf("uo3d") >= 0 || fmt.IndexOf("uo:3d") >= 0)
                {
                    m_Type = ClientType.UOTD;
                }
                else
                {
                    m_Type = ClientType.Regular;
                }
                #endregion
			}
			catch
			{
				m_Major = 0;
				m_Minor = 0;
				m_Revision = 0;
				m_Patch = 0;
				m_Type = ClientType.Regular;
			}
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}

			var o = obj as ClientVersion;

			if (o == null)
			{
				throw new ArgumentException();
			}

			if (m_Major > o.m_Major)
			{
				return 1;
			}
			else if (m_Major < o.m_Major)
			{
				return -1;
			}
			else if (m_Minor > o.m_Minor)
			{
				return 1;
			}
			else if (m_Minor < o.m_Minor)
			{
				return -1;
			}
			else if (m_Revision > o.m_Revision)
			{
				return 1;
			}
			else if (m_Revision < o.m_Revision)
			{
				return -1;
			}
			else if (m_Patch > o.m_Patch)
			{
				return 1;
			}
			else if (m_Patch < o.m_Patch)
			{
				return -1;
			}
			else
			{
				return 0;
			}
		}

		public static bool IsNull(object x)
		{
			return ReferenceEquals(x, null);
		}

		public int Compare(object x, object y)
		{
			if (IsNull(x) && IsNull(y))
			{
				return 0;
			}
			else if (IsNull(x))
			{
				return -1;
			}
			else if (IsNull(y))
			{
				return 1;
			}

			var a = x as ClientVersion;
			var b = y as ClientVersion;

			if (IsNull(a) || IsNull(b))
			{
				throw new ArgumentException();
			}

			return a.CompareTo(b);
		}

		public static int Compare(ClientVersion a, ClientVersion b)
		{
			if (IsNull(a) && IsNull(b))
			{
				return 0;
			}
			else if (IsNull(a))
			{
				return -1;
			}
			else if (IsNull(b))
			{
				return 1;
			}

			return a.CompareTo(b);
		}

        public static ClientVersion ConvertToRegularVersion(ClientVersion inVersion)
        {
            if (inVersion.Major >= 67) //Enhanced Client
            {
                //TODO:
                return new ClientVersion(inVersion.Major, inVersion.Minor, inVersion.Revision, inVersion.Patch, ClientType.EC);
            }
            else if ((inVersion.Major == 66) && (inVersion.Minor == 55)) //Kingdom Reborn Client
            {
                if (inVersion.Revision >= 53)
                {
                    return new ClientVersion(6, 0, 14, 2, ClientType.KR); //2.59.0.2
                }
                switch (inVersion.Revision)
                {
                    case 52:
                        return new ClientVersion(6, 0, 13, 1, ClientType.KR); //2.58.0.7
                    case 51:
                        return new ClientVersion(6, 0, 12, 4, ClientType.KR); //2.57.0.17
                    case 50:
                        return new ClientVersion(6, 0, 11, 0, ClientType.KR); //2.56.0.6
                    case 49:
                        return new ClientVersion(6, 0, 10, 0, ClientType.KR); //2.55.0.3 
                    case 48:
                        return new ClientVersion(6, 0, 9, 2, ClientType.KR); //2.54.0.7
                    case 47:
                        return new ClientVersion(6, 0, 8, 0, ClientType.KR); //2.53.0.2
                    case 46:
                        return new ClientVersion(6, 0, 7, 0, ClientType.KR); //2.52.0.4
                    case 45:
                        return new ClientVersion(6, 0, 6, 2, ClientType.KR); //2.51.0.3
                    case 44:
                        return new ClientVersion(6, 0, 5, 0, ClientType.KR); //2.48.0.7
                    case 43:
                        return new ClientVersion(6, 0, 4, 0, ClientType.KR); //2.47.1.10
                    case 42:
                        return new ClientVersion(6, 0, 3, 0, ClientType.KR); //2.47.0.8
                    case 41:
                        return new ClientVersion(6, 0, 2, 0, ClientType.KR); //2.46.1.9
                    case 40:
                        return new ClientVersion(6, 0, 1, 10, ClientType.KR); //2.46.1.6
                    /*case 35:
                        return new ClientVersion(6, 0, 1, 9, ClientType.KR); //2.45.7.3
                    case 34:
                        return new ClientVersion(6, 0, 1, 8, ClientType.KR); //2.45.6.3
                    case 33:
                        return new ClientVersion(6, 0, 1, 7, ClientType.KR); //2.45.5.6
                    case 32:
                        return new ClientVersion(6, 0, 1, 7, ClientType.KR); //2.45.5.4
                    case 31:
                        return new ClientVersion(6, 0, 1, 6, ClientType.KR); //2.45.4.4
                    case 30:
                        return new ClientVersion(6, 0, 1, 6, ClientType.KR); //2.45.4.3
                    case 29:
                        return new ClientVersion(6, 0, 1, 6, ClientType.KR); //2.45.4.2 //////
                    case 28:
                        return new ClientVersion(6, 0, 1, 5, ClientType.KR); //2.45.3.13
                    case 27:
                        return new ClientVersion(6, 0, 1, 5, ClientType.KR); //2.45.3.11
                    case 26:
                        return new ClientVersion(6, 0, 1, 5, ClientType.KR); //2.45.3.10
                    case 25:
                        return new ClientVersion(6, 0, 1, 4, ClientType.KR); //2.45.3.3
                    case 24:
                        return new ClientVersion(6, 0, 1, 4, ClientType.KR); //2.45.1.5
                    case 23:
                        return new ClientVersion(6, 0, 1, 4, ClientType.KR); //2.45.1.2
                    case 22:
                        return new ClientVersion(6, 0, 1, 3, ClientType.KR); //2.45.0.6
                    case 21:
                        return new ClientVersion(6, 0, 1, 3, ClientType.KR); //2.45.0.3
                    case 20:
                        return new ClientVersion(6, 0, 1, 2, ClientType.KR); //2.44.0.25
                    case 19:
                        return new ClientVersion(6, 0, 1, 1, ClientType.KR); //2.44.0.15*/
                    default:
                        return new ClientVersion(6, 0, 0, 0, ClientType.KR); //First KR Client 
                }
            }
            else
            {
                return inVersion;
            }
        }

    }
}