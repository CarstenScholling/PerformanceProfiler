page 98991 "Performance Profiler SubPage"
{
    Caption = 'Performance Profiler SubPage';
    PageType = ListPart;
    SourceTable = "Performance Profiler Line";
    UsageCategory = None;

    layout
    {
        area(Content)
        {
            repeater(General)
            {
                IndentationColumn = Rec.Identation;
                ShowAsTree = true;
                IndentationControls = ObjectTypeCtrl, ObjectName;

                field(EntryNo; Rec."Entry No.")
                {
                    ApplicationArea = Basic, Suite;
                    Editable = false;
                    Visible = false;
                    ToolTip = 'Specifies the consecutive number of this entry.';
                }

                field(ObjectTypeCtrl; Rec."Object Type")
                {
                    ApplicationArea = Basic, Suite;
                    Editable = false;
                    Visible = false;
                    ToolTip = 'Specifies the object type which raised this event.';
                }

                field(ObjectId; Rec."Object Id")
                {
                    ApplicationArea = Basic, Suite;
                    Editable = false;
                    Visible = false;
                    ToolTip = 'Specifies the object id which raised this event.';
                }

                field(ObjectName; Rec."Object Name")
                {
                    ApplicationArea = Basic, Suite;
                    Editable = false;
                    ToolTip = 'Specifies the object name which raised this event.';
                }

                field(Statement; Rec.Statement)
                {
                    ApplicationArea = Basic, Suite;
                    Editable = false;
                    ToolTip = 'Specifies the AL or SQL statement for this event.';
                }

                field(StatementLineNo; Rec."Statement Line No.")
                {
                    ApplicationArea = Basic, Suite;
                    Editable = false;
                    Visible = false;
                    ToolTip = 'Specifies the source line number of the statement.';
                }

                field(AppName; Rec."App Name")
                {
                    ApplicationArea = Basic, Suite;
                    Editable = false;
                    ToolTip = 'Specifies the App name this event originates from.';
                }

                field(UserName; Rec."User Name")
                {
                    ApplicationArea = Basic, Suite;
                    Editable = false;
                    Visible = false;
                    ToolTip = 'Specifies the user name executing the process causing the event.';
                }

                field(HitCount; Rec."Hit Count")
                {
                    ApplicationArea = Basic, Suite;
                    Editable = false;
                    ToolTip = 'Specifies the number of hits for this line/statament.';
                }

                field(DurationMs; Rec."Duration (ms)")
                {
                    ApplicationArea = Basic, Suite;
                    Editable = false;
                    ToolTip = 'Specifies the total duration in milliseconds for this statement. This is the cumulative sum of all durations.';
                }

                field(MinDuration; Rec."Min. Duration")
                {
                    ApplicationArea = Basic, Suite;
                    Editable = false;
                    Visible = false;
                    ToolTip = 'Specifies the minimum duration in milliseconds for this statement. Multiple executions of the same statement may have different execution times based on the system condition.';
                }

                field(MaxDuration; Rec."Max. Duration")
                {
                    ApplicationArea = Basic, Suite;
                    Editable = false;
                    Visible = false;
                    ToolTip = 'Specifies the maximum duration in milliseconds for this statement. Multiple executions of the same statement may have different execution times based on the system condition.';
                }

                field(LastActiveMs; Rec."Last Active (ms)")
                {
                    ApplicationArea = Basic, Suite;
                    Editable = false;
                    Visible = false;
                    ToolTip = 'I have no idea :)';
                }

                field(Tenant; Rec.Tenant)
                {
                    ApplicationArea = Basic, Suite;
                    Editable = false;
                    Visible = false;
                    ToolTip = 'Specifies the tenant for which this event was raised.';
                }

                field(SessionId; Rec."Session Id")
                {
                    ApplicationArea = Basic, Suite;
                    Editable = false;
                    Visible = false;
                    ToolTip = 'Specifies the session id in which this event was raised.';
                }
            }
        }
    }

    actions
    {
        area(Processing)
        {
            action(ShowStatement)
            {
                ApplicationArea = Basic, Suite;
                Caption = 'Show Statement';
                ToolTip = 'Use this action to show the full statement.';
                Image = ShowList;
                Promoted = true;
                PromotedIsBig = true;
                PromotedCategory = Process;
                PromotedOnly = true;

                trigger OnAction()
                begin
                    Message(Rec.GetStatement());
                end;
            }
        }
    }
}