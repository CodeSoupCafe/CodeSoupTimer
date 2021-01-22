namespace LinearTimeCodeGenerator.LTCSharper
{
  using System;

  [Flags]
  public enum BGFlags
  {
    NONE = 1 << 0,
    USE_DATE = 1 << 1,
    TC_CLOCK = 1 << 2,
    BGF_DONT_TOUCH = 1 << 3,
    NO_PARITY = 1 << 4
  };
}
