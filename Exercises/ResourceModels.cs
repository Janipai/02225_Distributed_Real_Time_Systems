using _02225.Entities;

namespace _02225;

public class ResourceModels
{
    // PRM: Periodic Resource Model (Θ, Π)
    public (double theta, double pi) ComputePRMInterface(List<ProjectTask> tasks)
    {
        // Similar to BDR but with periodic supply bursts
        // 1. Calculate total demand over hyperperiod
        long hyperperiod = CalculateHyperperiod(tasks);
        double totalDemand = tasks.Sum(t => 
            (hyperperiod / t.Period) * t.Wcet
        );

        // 2. Find smallest Π where Θ/Π >= total utilization
        double minUtil = tasks.Sum(t => (double)t.Wcet / t.Period);
        double pi = tasks.Min(t => t.Period); // Start with smallest task period
    
        // Iterative search for optimal (Θ, Π)
        double theta = totalDemand * pi / hyperperiod;
        while (true)
        {
            if (theta / pi >= minUtil - 1e-9)
                break;
        
            pi *= 0.9; // Reduce period (conservative)
            theta = totalDemand * pi / hyperperiod;
        }

        return (Math.Ceiling(theta), pi); // Round up Θ for safety
    }

    // EDP: Explicit Deadline Periodic Model (Θ, Π, Δ)
    public (double theta, double pi, double delta) ComputeEDPInterface(List<ProjectTask> tasks)
    {
        // Combines PRM with explicit deadlines
        // 1. First compute PRM parameters as base
        var (theta, pi) = ComputePRMInterface(tasks);

        // 2. Calculate additional deadline slack (Δ)
        double delta = 0;
        foreach (var t in tasks)
        {
            double taskSlack = t.Period - t.Wcet / (theta / pi);
            delta = Math.Max(delta, taskSlack);
        }

        // 3. Adjust Θ to account for slack
        theta = tasks.Sum(t => t.Wcet + (delta * (theta / pi)));
        
        return (theta, pi, delta);
    }
    
    // BDR: Explicit Deadline Periodic Model (alpha, beta)
    public (double alpha, double delta) ComputeBDRInterface(List<ProjectTask> tasks)
    {
        // Start with the total utilization as a lower bound for α
        double totalUtil = tasks.Sum(task => task.Wcet / task.Period);
        double alphaMin = Math.Min(totalUtil, 1.0);
        if (alphaMin < 0) alphaMin = 0; // just in case (shouldn’t happen, tasks have positive execution times)

        // If the task set is not even schedulable on a full processor, we handle that as a special case.
        // Check schedulability on a full core (α=1, Δ=0) by basic demand criterion:
        if (!IsDemandSchedulable(tasks, 1.0, 0.0))
        {
            // If tasks miss deadlines even on a full core, we cannot find a valid (α, Δ).
            // Here we simply return full resource and note a large delay as a placeholder.
            return (1.0, 0.0);
        }

        // Binary search for minimal alpha in [alphaMin, 1] that yields a feasible Δ
        double lo = alphaMin;
        double hi = 1.0;
        double bestAlpha = 1.0;
        double bestDelta = 0.0;

        // Tolerance for binary search (stop when interval is very small)
        const double eps = 1e-3;
        while (hi - lo > eps)
        {
            double mid = (lo + hi) / 2.0;
            double requiredDelta = ComputeRequiredDelay(tasks, mid);
            if (requiredDelta >= 0)
            {
                // Feasible: supply mid*CPU with delay = requiredDelta meets demand
                bestAlpha = mid;
                bestDelta = requiredDelta;
                // Try to reduce alpha further
                hi = mid;
            }
            else
            {
                // Not feasible: increase alpha
                lo = mid;
            }
        }

        // After search, bestAlpha/bestDelta holds the minimal interface found
        // We round Δ up to the next higher non-negative value if needed (it should be >=0 already if feasible).
        if (bestDelta < 0) bestDelta = 0.0;
        return (bestAlpha, bestDelta);
    }


// Shared utility for all models
private long CalculateHyperperiod(List<ProjectTask> tasks)
{
    return tasks.Select(t => (long)t.Period)
        .Aggregate(1L, (a, b) => Lcm(a, b));
}

// Helper: Compute least common multiple of two numbers
private long Lcm(long a, long b)
{
    return a / Gcd(a, b) * b;
}

private long Gcd(long a, long b)
{
    return b == 0 ? Math.Abs(a) : Gcd(b, a % b);
}

// Helper: Compute the demand bound function for a task set at time t
private double ComputeDbf(List<ProjectTask> tasks, double t)
{
    double demand = 0.0;
    foreach (ProjectTask task in tasks)
    {
        // Get task parameters
        double C = task.Wcet; // worst-case execution time
        double T = task.Period;
        double D = task.Period; // use period if no explicit deadline

        if (t < D)
        {
            // No job of this task has deadline <= t
            continue;
        }

        // Number of jobs with deadlines <= t = floor((t + T - D) / T)
        double count = Math.Floor((t + T - D) / T);
        if (count < 0) count = 0;
        demand += count * C;
    }

    return demand;
}

// Helper: Determine if a task set is schedulable on a resource with given α and Δ (using demand bound test)
    private bool IsDemandSchedulable(List<ProjectTask> tasks, double alpha, double delta)
    {
        // We need to verify that for all t, SBF(α,Δ)(t) >= DBF(t).
        // SBF(α,Δ)(t) for bounded-delay resource is 0 for t < Δ, and α * (t - Δ) for t >= Δ.
        // So we must check DBF(t) <= α*(t - Δ) for all t >= Δ (and trivially satisfied for t < Δ if no demand before Δ).
        // It suffices to check at critical points (task deadlines).
        double horizon = ComputeAnalysisHorizon(tasks);
        // Check at t = Δ as well (just after supply starts)
        double startT = delta;
        if (startT < 0) startT = 0;
        // We iterate over relevant time points (all tasks' deadlines up to horizon)
        foreach (double t in GetCriticalTimes(tasks, horizon))
        {
            if (t < delta)
                continue; // before resource starts supplying, no supply (and ideally no demand due to no deadlines <= t)
            double demand = ComputeDbf(tasks, t);
            double supply = alpha * (t - delta);
            if (demand > supply + 1e-9)
            {
                // demand exceeds supply at time t, not schedulable
                return false;
            }
        }

        return true;
    }

    // Helper: Compute the minimum required delay Δ for a given α such that DBF <= SBF(α, Δ)
    private double ComputeRequiredDelay(List<ProjectTask> tasks, double alpha)
    {
        // If α is 0 (no resource provided), tasks can't be served (return -∞ as impossible)
        if (alpha <= 0) return double.NegativeInfinity;

        double horizon = ComputeAnalysisHorizon(tasks);
        double minOffset = double.PositiveInfinity;
        // We want Δ such that for all t: α*(t - Δ) >= DBF(t), or equivalently Δ <= t - (DBF(t)/α).
        // So we find the minimal right-hand side across all t in [0, horizon].
        foreach (double t in GetCriticalTimes(tasks, horizon))
        {
            double dbf = ComputeDbf(tasks, t);
            // Rearranged condition: Δ <= t - dbf/α
            double rhs = t - (dbf / alpha);
            if (rhs < minOffset)
            {
                minOffset = rhs;
            }
        }

        // The maximum needed Δ would be the negative of minOffset if minOffset is negative.
        // Actually, if minOffset is positive, we can choose Δ = minOffset (the smallest t - dbf/α).
        // If minOffset is negative, it means even at t = 0, demand vs supply condition fails (which usually implies α is too low).
        // Return minOffset as the feasible Δ (if minOffset < 0, it indicates infeasible).
        return minOffset;
    }
    
    

    // Helper: Determine a safe analysis horizon (time limit) to check for demand vs supply
    private double ComputeAnalysisHorizon(List<ProjectTask> tasks)
    {
        // A conservative choice is the hyperperiod (LCM of all periods) plus the maximum deadline.
        // This covers all possible release alignments&#8203;:contentReference[oaicite:9]{index=9}.
        long lcm = 1;
        long maxDeadline = 0;
        foreach (ProjectTask task in tasks)
        {
            long T = task.Period;
            lcm = Lcm(lcm, T);
            long D = task.Period;
            if (D > maxDeadline) maxDeadline = D;
        }

        long horizon = lcm + maxDeadline;
        // Limit horizon to avoid extreme values (if LCM is huge, we rely on utilization and demand pattern analysis).
        // For safety, cap at some large value if needed (e.g., 1000 * max period).
        long cap = 1000 * tasks.Max(task => (long)task.Period);
        if (horizon > cap) horizon = cap;
        return (double)horizon;
    }

    // Helper: Collect all critical time points (deadlines of jobs) up to the given horizon
    private IEnumerable<double> GetCriticalTimes(List<ProjectTask> tasks, double horizon)
    {
        var timePoints = new SortedSet<double>();
        timePoints.Add(0);
        foreach (ProjectTask task in tasks)
        {
            double T = task.Period;
            double D = task.Period;
            // Generate deadlines for jobs of this task up to horizon
            if (T <= 0) continue;
            for (double t = D; t <= horizon; t += T)
            {
                timePoints.Add(Math.Round(t, 6)); // round to avoid floating precision issues
            }
        }

        return timePoints;
    }
}