namespace LinearTimeCodeGenerator.LTCSharper
{
  public struct FrameData
  {
    uint frame_units; ///< SMPTE framenumber BCD unit 0..9
		uint user1;

    uint frame_tens; ///< SMPTE framenumber BCD tens 0..3
		uint dfbit; ///< indicated drop-frame timecode
		uint col_frame; ///< colour-frame: timecode intentionally synchronized to a colour TV field sequence
		uint user2;

    uint secs_units; ///< SMPTE seconds BCD unit 0..9
		uint user3;

    uint secs_tens; ///< SMPTE seconds BCD tens 0..6
		uint biphase_mark_phase_correction; ///< see note on Bit 27 in description and \ref ltc_frame_set_parity .
		uint user4;

    uint mins_units; ///< SMPTE minutes BCD unit 0..9
		uint user5;

    uint mins_tens; ///< SMPTE minutes BCD tens 0..6
		uint binary_group_flag_bit0; ///< indicate user-data char encoding, see table above - bit 43
		uint user6;

    uint hours_units; ///< SMPTE hours BCD unit 0..9
		uint user7;

    uint hours_tens; ///< SMPTE hours BCD tens 0..2
		uint binary_group_flag_bit1; ///< indicate timecode is local time wall-clock, see table above - bit 58
		uint binary_group_flag_bit2; ///< indicate user-data char encoding (or parity with 25fps), see table above - bit 59
		uint user8;

    uint sync_word;
  };
}
