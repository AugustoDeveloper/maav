export class BranchActionRequest {
    public to: string;
    public from: string;
    public commit: string;
    public message: string;
    public preReleaseLabel: string;
    public buildLabel: string;
}