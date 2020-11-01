page 98993 "Perf. Profiler Archive List"
{
    Caption = 'Performance Profiler Archive List';
    PageType = List;
    SourceTable = "Performance Profiler Archive";
    ApplicationArea = Basic, Suite;
    UsageCategory = Administration;

    layout
    {
        area(content)
        {
            repeater(General)
            {
                field(Code; Rec.Code)
                {
                    ApplicationArea = Basic, Suite;
                    ToolTip = 'Specifies a Code to identify the performance analysis session.';
                }

                field(Description; Rec.Description)
                {
                    ApplicationArea = Basic, Suite;
                    ToolTip = 'Specifies the Description of the performance analysis session.';
                }

                field(NoOfLines; "No. of Lines")
                {
                    ApplicationArea = Basic, Suite;
                    ToolTip = 'Specifies the number of lines already collected for this profiler entry.';
                }
            }
        }
    }
}
