page 98992 "Performance Profiler List"
{
    Caption = 'Performance Profiler List';
    PageType = List;
    SourceTable = "Performance Profiler";
    CardPageId = "Performance Profiler";
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

    actions
    {
        area(Navigation)
        {
            action(Archive)
            {
                ApplicationArea = Basic, Suite;
                Caption = 'Archive';
                ToolTip = 'Use this action to show the list of archived recording sessions.';
                Image = Archive;
                RunObject = page "Perf. Profiler Archive List";
            }
        }

        area(Processing)
        {
            action(CopyToArchive)
            {
                ApplicationArea = Basic, Suite;
                Caption = 'Copy to Archive';
                ToolTip = 'Use this action to copy the current profiler session to the archive.';
                Image = Copy;

                trigger OnAction()
                begin
                    Rec.CopyToArchive(false);
                    CurrPage.Update(false);
                end;
            }

            action(CopyFromArchive)
            {
                ApplicationArea = Basic, Suite;
                Caption = 'Copy from Archive';
                ToolTip = 'Use this action to copy an archived profiler session back.';
                Image = Copy;

                trigger OnAction()
                begin
                    Rec.CopyFromArchive(false);
                    CurrPage.Update(false);
                end;
            }
        }
    }
}
