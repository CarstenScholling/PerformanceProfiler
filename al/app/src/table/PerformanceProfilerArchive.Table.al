table 98992 "Performance Profiler Archive"
{
    Caption = 'Performance Profiler Archive';
    DataClassification = CustomerContent;
    LookupPageId = "Perf. Profiler Archive List";
    DrillDownPageId = "Perf. Profiler Archive List";

    fields
    {
        field(10; Code; Code[20])
        {
            Caption = 'Code';
            DataClassification = CustomerContent;
        }

        field(100; Description; Text[100])
        {
            Caption = 'Description';
            DataClassification = CustomerContent;
        }

        field(110; "No. of Lines"; Integer)
        {
            Caption = 'No. of Lines';
            BlankZero = true;
            Editable = false;
            FieldClass = FlowField;
            CalcFormula = count("Performance Profiler Line" where("Performance Profiler Code" = field(Code)));
        }

        field(200; "Session Id"; Integer)
        {
            Caption = 'Session ID';
            BlankZero = true;
            MinValue = -1;
            TableRelation = "Active Session"."Session ID";
            ValidateTableRelation = false;

            trigger OnLookup()
            var
                activeSession: Record "Active Session";
            begin
                if Page.RunModal(Page::"Concurrent Session List", activeSession) = Action::LookupOK then begin
                    "Session Id" := activeSession."Session ID";
                end;
            end;
        }

        field(210; Threshold; Integer)
        {
            Caption = 'Threshold';
            BlankZero = true;
            MinValue = 0;
        }
    }

    keys
    {
        key(PK; Code)
        {
            Clustered = true;
        }
    }

    fieldgroups
    {
        fieldgroup(DropDown; Code, Description)
        {
        }
    }

    trigger OnDelete()
    begin
        DeleteRelatedTables();
    end;

    local procedure DeleteRelatedTables()
    var
        performanceProfilerLineArchive: Record "Perf. Profiler Line Archive";
    begin
        performanceProfilerLineArchive.SetFilter("Performance Profiler Code", '%1', Code);
        performanceProfilerLineArchive.DeleteAll();
    end;
}
