page 98990 "Performance Profiler"
{
    Caption = 'Performance Profiler';
    PageType = Document;
    SourceTable = "Performance Profiler";
    UsageCategory = None;

    layout
    {
        area(Content)
        {
            group(General)
            {
                group(Left)
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

                    field(NoOfLines; Rec."No. of Lines")
                    {
                        ApplicationArea = Basic, Suite;
                        ToolTip = 'Specifies the number of lines already collected for this profiler entry.';
                    }
                }

                group(Right)
                {
                    field(RecordThreshold; Rec.Threshold)
                    {
                        ApplicationArea = Basic, Suite;
                        ToolTip = 'Specifies the threshold to use for event recording.';
                        Enabled = not ProfilerIsRunning;
                    }

                    field(RecordSessionId; Rec."Session Id")
                    {
                        ApplicationArea = Basic, Suite;
                        ToolTip = 'Specifies the session ID to record.';
                        Enabled = not ProfilerIsRunning;
                    }

                    field(RecordAppName; Rec."App Name")
                    {
                        ApplicationArea = Basic, Suite;
                        ToolTip = 'Specifies the App to filter.';
                        Enabled = not ProfilerIsRunning;
                    }
                }
            }

            part(Lines; "Performance Profiler SubPage")
            {
                ApplicationArea = Basic, Suite;
                Caption = 'Lines';
                SubPageLink = "Performance Profiler Code" = field(Code);
            }
        }
    }

    actions
    {
        area(Processing)
        {
            action(StartRecord)
            {
                ApplicationArea = Basic, Suite;
                Caption = 'Start';
                ToolTip = 'Use this action to start recording for the specified session and with the given threshold.';
                Image = Start;
                Promoted = true;
                PromotedIsBig = true;
                PromotedCategory = Process;
                PromotedOnly = true;
                Enabled = not ProfilerIsRunning;

                trigger OnAction()
                begin
                    PerformanceProfilerMgt.Start(Rec);
                    CurrPage.Update(false);
                end;
            }

            action(StopRecord)
            {
                ApplicationArea = Basic, Suite;
                Caption = 'Stop';
                ToolTip = 'Use this action to stop the recording.';
                Image = Stop;
                Promoted = true;
                PromotedIsBig = true;
                PromotedCategory = Process;
                PromotedOnly = true;
                Enabled = ProfilerIsRunning;

                trigger OnAction()
                begin
                    ProfilerIsRunning := false;
                    CurrPage.Update(false);

                    PerformanceProfilerMgt.Stop();
                end;
            }

            action(AnalyeEtlFile)
            {
                ApplicationArea = Basic, Suite;
                Caption = 'Analyze ETL File';
                ToolTip = 'Use this action to analyze an already recorded ETL file.';
                Image = AnalysisView;
                Promoted = true;
                PromotedIsBig = true;
                PromotedCategory = Process;
                PromotedOnly = true;
                Enabled = not ProfilerIsRunning;

                trigger OnAction()
                begin
                    PerformanceProfilerMgt.AnalyzeEtlFile(Rec.Code, Rec.Threshold);
                    CurrPage.Update(false);
                end;
            }
        }
    }

    var
        PerformanceProfilerMgt: Codeunit "Performance Profiler Mgt.";
        [InDataSet]
        ProfilerIsRunning: Boolean;

    trigger OnAfterGetCurrRecord()
    begin
        ProfilerIsRunning := PerformanceProfilerMgt.IsRunning();
        "Session Id" := SessionId();
    end;
}
