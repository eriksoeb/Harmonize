# An AI-Friendly JSON Representation for Official Time Series: Design Principles and Trade-offs for Next-Generation Statistical Dissemination

---

**[Author Name]**
[Affiliation, Address]
[Email]

---

## Abstract

The rapid adoption of large language models (LLMs) and AI agents as instruments of data retrieval and analysis introduces new functional requirements for statistical dissemination systems. Existing formats — principally SDMX, JSON-stat, and PX-Web — were designed for institutional interoperability and multidimensional reporting, not for the lightweight, self-contained, machine-native consumption patterns characteristic of AI-driven workflows. This paper examines these requirements systematically, identifies specific structural features of current formats that impede AI-era consumption, and proposes a compact JSON representation for statistical time series built on epoch timestamps (milliseconds elapsed since 1970-01-01 00:00:00 UTC), minimal mandatory structure, and embedded metadata. We compare the proposed format against existing standards across dimensions of parsing complexity, payload efficiency, semantic expressiveness, and interoperability. We argue that the format is best positioned as a complementary dissemination layer, not a replacement for institutional standards, and that its adoption could reduce the structural selection effect that currently causes AI systems to bypass authoritative official sources in favor of simpler but less rigorous alternatives. We additionally examine the role of Model Context Protocol (MCP) servers as an emerging interface layer for AI data access, and argue that MCP is most effective when built on top of well-structured data — not as a substitute for it. Implications for NSO dissemination strategy are discussed.

**Keywords:** statistical dissemination, artificial intelligence, large language models, time series, JSON, SDMX, JSON-stat, epoch timestamps, MCP, interoperability, machine-readability

---

## 1. Introduction

Statistical dissemination has historically evolved in response to the dominant mode of data consumption. The transition from paper publications to CD-ROM databases, and from FTP bulk downloads to interactive web APIs, each required re-examination of format design and metadata conventions (Sundgren, 2007). The emergence of large language models (LLMs) and AI agents as active consumers of statistical data represents a comparable transition — one that the statistical community has not yet fully addressed at the level of format design.

AI systems that retrieve and process data operate under constraints that differ fundamentally from those of human users or institutional exchange partners. Where human users benefit from navigational interfaces and institutional conventions, and where SDMX clients benefit from full schema resolution and governance support, AI agents optimize for minimal parsing overhead, self-contained responses, and immediate computational usability. When these conditions are not met, AI systems — whether by design or emergent behavior — select alternative data sources that impose lower structural cost, even when those sources are less authoritative.

This selection effect has practical consequences. National statistical offices (NSOs) invest substantially in producing timely, high-quality, methodologically rigorous data. If AI-driven workflows systematically bypass these sources in favor of simpler but less rigorous aggregators, the statistical community's mission of supporting evidence-based decision-making is compromised at the point of consumption, regardless of the quality of production.

This paper makes the following contributions:

1. A systematic analysis of the structural features of SDMX, JSON-stat, and PX-Web that create obstacles for AI-era consumption.
2. A proposed compact JSON format for time series dissemination, with explicit design rationale for each structural choice.
3. A comparative assessment of the proposed format against existing standards across multiple dimensions.
4. A conceptual discussion of the token and payload cost implications of format choices for LLM-based pipelines.
5. Design principles for NSO dissemination strategy in the context of AI-era consumption patterns.

The paper proceeds as follows. Section 2 reviews the background literature on statistical dissemination formats and the emerging literature on AI data consumption. Section 3 analyses the limitations of existing formats for AI-era use cases. Section 4 presents the proposed format and its design principles. Section 5 provides a comparative assessment. Section 6 discusses implications, limitations, and future directions. Section 7 concludes.

---

## 2. Background and Related Work

### 2.1 The evolution of statistical dissemination formats

Statistical dissemination has moved through several format generations, each reflecting the dominant consumption context of its era. The PC-Axis format, introduced by Statistics Sweden in the 1990s, was designed for batch download and desktop tools (Statistics Sweden, 2020). SDMX, developed jointly by seven international organizations beginning in 2001, addressed the need for structured, governed, interoperable exchange of multidimensional statistical data (BIS et al., 2005; Eurostat, 2020).

JSON-stat, proposed by Badosa (2012) and adopted by several NSOs including Statistics Norway, represents an earlier effort to reduce dissemination complexity. It serializes statistical tables as compact JSON structures with dimension arrays and value vectors. While substantially simpler than SDMX-ML in terms of verbosity, JSON-stat retains a tabular, dimension-oriented data model that was designed for cross-sectional queries rather than time series retrieval.

The SDMX 2.1 specification introduced SDMX-JSON as a JSON serialization of the SDMX information model (SDMX Global Conference, 2021), reducing payload size relative to SDMX-ML while preserving the full SDMX semantic model. SDMX-JSON is thus more compact than its XML counterpart but retains the same structural complexity.

### 2.2 AI systems as data consumers

Large language models with tool-use capabilities — including function calling and API retrieval — represent a qualitatively new class of data consumer. These systems can be instructed to retrieve data from external sources, reason over retrieved observations, and present results in natural language (Brown et al., 2020; OpenAI, 2023; Anthropic, 2024). The deployment of such systems in analytical contexts is growing rapidly, with applications in automated report generation, exploratory data analysis, and research assistance (Wei et al., 2022; Schick et al., 2023).

The behavior of AI agents in data retrieval is not yet well-documented in the statistical literature, but several properties are well-established from the machine learning literature and practitioner experience. AI agents with access to multiple data sources exhibit preferential behavior toward sources that return self-contained, immediately parseable responses (Schick et al., 2023). Deeply nested or schema-dependent formats increase reasoning token cost — the number of computational steps required before data values can be extracted and used — and increase the probability of misinterpretation (Wei et al., 2022).

This implies a practical selection effect: AI agents, all else equal, will prefer World Bank or FRED API endpoints over SDMX endpoints, because the former return compact JSON time series with minimal parsing requirements, while the latter require schema resolution and frequency-specific time period parsing before data can be used. The consequences of this selection effect for the use of official statistics in AI-driven analytical workflows motivate the present investigation.

### 2.3 Time representation in statistical formats

A recurring technical challenge in statistical dissemination is the representation of observation time. SDMX and related formats use frequency-specific period notation: `2025-Q1` for quarterly data, `2025W53` for weekly data (subject to ISO week number conventions), `2025M12` for monthly data. These representations require knowledge of the series frequency before the time dimension can be correctly parsed and ordered.

This is non-trivial in mixed-frequency contexts — for instance, when aligning a monthly price index series with a quarterly national accounts series and a weekly financial indicator. Correct alignment requires frequency-specific parsing rules, calendar convention resolution (ISO week definitions vary across systems), and explicit handling of reference period conventions (beginning, middle, or end of period).

Unix timestamps, defined as the number of milliseconds elapsed since 1970-01-01 00:00:00 UTC (ISO, 2019; IEEE, 2017), are unambiguous integers that require no frequency-specific interpretation. They are natively sortable, directly comparable across series of different frequencies, and universally supported in modern computing environments. Their principal limitation — reduced direct human readability — is increasingly mitigated by the prevalence of conversion tools, visualization libraries, and AI-driven interpretation interfaces.

---

## 3. Limitations of Current Formats for AI-Era Consumption

### 3.1 SDMX

SDMX's power for institutional exchange rests on features that create overhead for lightweight consumption. A correct SDMX client must:

1. Retrieve the data structure definition (DSD) for the relevant dataflow.
2. Resolve concept schemes and code lists referenced by the DSD.
3. Map dimension and attribute values using the resolved code lists.
4. Parse time period strings using frequency-specific rules.
5. Construct the data matrix from the intersection of dimension values.

Steps 1–3 typically require additional HTTP requests before the data response itself can be interpreted. This multi-step resolution process is well-suited to institutional clients with persistent DSD caches but is poorly suited to stateless AI agents making single-query data retrievals.

A further obstacle is the SDMX time period notation, which is proprietary and does not conform to ISO 8601. Representations such as `2025-Q1`, `2025W53`, and `2025M12` cannot be parsed or sorted without frequency-specific logic. Crucially, the parsing rules differ across frequencies: a weekly period requires ISO week-to-date resolution, a quarterly period requires quarter-boundary expansion, and a monthly period requires month-start or month-end convention decisions that are typically implicit. When a consumer needs to compare or align series of different frequencies, each frequency type demands its own conversion path, compounding implementation complexity and error risk at every integration point.

The token cost implication is significant. For LLM-based pipelines that pass retrieved data into context, a verbose SDMX-ML response for a single time series may consume thousands of tokens before a single numeric value is accessible to the model. This directly increases inference cost and may cause relevant data to be truncated in systems with fixed context windows.

### 3.2 JSON-stat

JSON-stat (Badosa, 2012) reduces verbosity substantially relative to SDMX-ML. However, its tabular, dimension-based structure introduces parsing requirements that are non-trivial for AI consumers. Extracting a simple time-value sequence from a JSON-stat response requires:

1. Identifying the time dimension among potentially many dimensions.
2. Reading the ordered values array for the time dimension.
3. Matching value indices to observation positions in the flat value vector.
4. Converting time labels (e.g., `"2025-01"`) using frequency-specific rules.

This is feasible for a programmed client but introduces additional reasoning steps for an AI agent that must infer the structure from context. Furthermore, JSON-stat does not specify a standard for embedding series-level metadata (unit, source, base year) in a retrievable single-series response, leaving producers to extend the format in non-standard ways.

### 3.3 PX-Web

PX-Web provides REST API access to PC-Axis databases and is widely deployed among Nordic NSOs (Statistics Sweden, 2020). Understanding its limitations for machine consumption requires understanding its design intent: PX-Web was built primarily as a human navigation interface. Its core interaction model — cascading dimension selectors, value filters, and table previews — assumes a person is operating the system. The REST API was added as a secondary capability layered on top of this human-oriented architecture; it was not designed from the ground up for programmatic or machine-native consumption.

This design origin has consequences that extend to the underlying storage layer. The PC-Axis flat-file format, on which PX-Web databases are built, was conceived more than three decades ago under computing constraints in which disk space was the binding resource. Its text-based, positionally encoded structure reflects optimization for compact storage rather than for retrieval speed, columnar access, or direct integration with modern analytical pipelines. Systems that prioritize low-latency query responses and streaming data access are structurally disadvantaged when the underlying storage model was not designed with those requirements in mind.

**Non-unique series identifiers.** A particularly significant limitation for AI and developer consumers is the absence of a consistent, globally unique series identifier in PX-Web. Retrieving a specific series requires knowing not only which table it belongs to, but also the names of the dimension variables defined within that specific table — names that are assigned independently by each statistical product and each NSO. A consumer querying consumer price data in one product may need to filter on a variable named `contents_code`; the equivalent dimension in another product from the same NSO may be named `consumer-groups`; and in a product from a different NSO it may be called `product_category`. There is no single key that identifies a series across tables or across national offices.

This contrasts directly with the identifier model used by simpler data services. The World Bank API assigns every indicator a single, globally unique code (e.g., `FP.CPI.TOTL`) that functions as a stable, system-independent reference. The proposed format adopts the same principle through its `name` field: a single identifier that is sufficient to retrieve the series without any prior knowledge of the source system's internal dimension structure. For AI agents and automated pipelines, this is not a cosmetic distinction — a stable unique name can be stored, communicated, and reused across systems, whereas a PX-Web series reference encodes a dependency on a specific table's internal structure that cannot be generalized.

**Time label inconsistency.** PX-Web responses use string-based time labels inherited from the PC-Axis format, and these labels are not standardized across statistical products or across NSOs. A consumer integrating PX-Web data from multiple national offices may encounter labels such as `"2025M01"`, `"2025-01"`, `"January 2025"`, or `"2025Q1"` for the same calendar period, each requiring different parsing logic. Every integration project must therefore implement its own time-label normalization layer — a cost that is multiplied across every developer, every AI system, and every data pipeline that attempts to work with PX-Web data at scale.

### 3.4 The shared date-time problem

The limitations identified in sections 3.1 and 3.3 share a common root: neither SDMX nor PX-Web adopts a universal, machine-native standard for representing observation time. Both rely on string-based, frequency-specific period notations that are neither directly sortable nor directly comparable across series without prior conversion.

The practical cost of this shared limitation accumulates at multiple levels. At the individual series level, each time label must be parsed and converted to a usable datetime representation before any computation can proceed. At the multi-series level, when comparing or aligning series of different frequencies — for example, a monthly price index with a quarterly national accounts aggregate — the consumer must additionally resolve reference period conventions (does a monthly observation belong to the start, midpoint, or end of the month?) that are typically implicit in metadata rather than explicitly encoded in the time label itself.

For AI agents, these conversion requirements represent a structural deterrent. An agent that receives period-notation time labels must devote reasoning steps to calendar interpretation before it can address the analytical question it was asked. This increases both token cost and the probability of error, and it creates a systematic preference for data sources that return unambiguous numeric timestamps — regardless of whether those sources are more or less authoritative than official statistical offices. Unix timestamps (milliseconds since 1970-01-01T00:00:00Z) resolve this problem completely: they are sortable integers, directly comparable across all frequencies, and require no calendar knowledge to use correctly.

---

## 4. A Proposed Lightweight JSON Format for Time Series Dissemination

### 4.1 Design objectives

The proposed format is designed to satisfy the following objectives:

1. **Self-containment**: A single API response must be fully interpretable without additional schema resolution requests.
2. **Machine-native time representation**: Time must be encoded as an unambiguous, directly sortable value.
3. **Frequency agnosticism**: The format must impose no constraint on observation frequency or regularity.
4. **Timezone correctness**: UTC storage with timezone-aware conversion at the query layer must be a first-class design property.
5. **Minimal mandatory structure**: Producers should be able to generate conformant responses with minimal implementation effort.
6. **Embedded essential metadata**: Unit, source, frequency, and base year must be co-located with data values.
7. **Extensibility**: Optional fields should not break consumers that do not recognize them.
8. **Interoperability with AI context windows**: Payload size should be minimized to reduce token cost in LLM-based pipelines.

### 4.2 Core structure

```json
{
  "series": [
    {
      "name": "CPI:C00.IDX",
      "unit": "INDEX",
      "title": "Consumer Price Index, All Items",
      "source": "Statistics Norway",
      "freq": "MONTHLY",
      "base_year": 2015,
      "last_updated": "2025-06-01",
      "data": [
        [1704067200000, 95.9],
        [1706745600000, 96.1],
        [1709251200000, 96.3]
      ]
    }
  ]
}
```

The `data` field contains an ordered array of two-element tuples `[timestamp_ms, value]`. Timestamps are Unix milliseconds in UTC. Missing observations are represented by `null` in the value position: `[1704067200000, null]`.

### 4.3 Mandatory and optional fields

**Mandatory fields:**
- `name` (string): Unique series identifier, recommended format `LOADSET:SERIESNAME`
- `unit` (string): Unit of measurement
- `data` (array): Ordered array of `[timestamp_ms, value]` tuples

**Recommended fields:**
- `title` (string): Human-readable series description
- `source` (string): Producing institution
- `freq` (string): One of `DAILY`, `WEEKLY`, `MONTHLY`, `QUARTERLY`, `ANNUAL`
- `base_year` (integer): Reference year for index series
- `last_updated` (string): ISO 8601 date of most recent update

**Optional rendering fields** (for direct visualization use):
- `type` (string): Chart type hint (e.g., `"spline"`, `"column"`)
- `color` (string): Hex color code
- `y_axis` (integer): Y-axis assignment in multi-axis charts
- `transform` (string): Pre-applied transformation code (e.g., `"pct12"` for 12-month percentage change)

Rendering fields are ignored by consumers that do not implement them.

### 4.4 Multi-series response

Multiple series can be returned in a single response, enabling mixed-frequency comparisons without multiple API calls:

```json
{
  "series": [
    {
      "name": "CPI:C00.IDX",
      "freq": "MONTHLY",
      "unit": "INDEX",
      "data": [[1704067200000, 95.9], [1706745600000, 96.1]]
    },
    {
      "name": "GDP:VOLUME",
      "freq": "QUARTERLY",
      "unit": "MEUR",
      "data": [[1704067200000, 12540.2], [1711929600000, 12688.5]]
    }
  ]
}
```

Because both series use Unix timestamps, they can be aligned on a common time axis without frequency-specific resolution logic.

The same property extends to variable-frequency and irregular observation schedules. Meteorological forecast data, for example, commonly transitions from hourly observations at short horizons to three-hourly, six-hourly, and twelve-hourly intervals as the forecast horizon extends — a pattern that cannot be cleanly represented in formats requiring a single declared frequency. Hydrological measurements, sensor readings, and event-driven observations similarly produce timestamps at irregular intervals determined by physical conditions rather than administrative calendars. In the proposed format, all of these cases are handled identically to conventional fixed-frequency statistical series: the timestamps are stored as they occur, and no frequency declaration is required for the data to be correctly interpreted. The `frequency` field, where present, is informational metadata for the consumer — the timestamps are always the authoritative time reference.

For official statistics, where most series carry fixed, declared frequencies, this is a structural property that eliminates an entire class of edge cases rather than a primary design motivation. For adjacent domains — environmental monitoring, energy systems, real-time financial data, and forecasting — it substantially broadens the format's applicability without any change to the core structure.

**Timezone handling and DST correctness.** A complementary advantage of UTC-based timestamp storage is unambiguous timezone representation. The proposed format stores all observations as UTC epoch values. Where local-time output is required — for instance, presenting Norwegian electricity consumption data in Central European Time for a domestic audience — timezone conversion is applied at the database query layer, using the full IANA timezone database supported by SQL Server and equivalent engines. This architecture correctly handles daylight saving time transitions, which represent a genuine data quality problem for sub-daily series stored in local time.

On the autumn clock-change night in Central European Time, the hour from 02:00 to 03:00 occurs twice: once in summer time (CEST, UTC+2) and once in winter time (CET, UTC+1). These two intervals correspond to distinct UTC epochs — `01:00 UTC` and `02:00 UTC` respectively — and a system storing observations in UTC correctly retains both readings as separate, unambiguous records. A system that stores or labels observations in local time cannot distinguish between the two: both carry the timestamp `02:00`, with no recoverable information about which belongs to the summer-time hour and which to the winter-time hour. For energy metering, meteorological recording, and transport monitoring — all domains where sub-hourly or hourly data is standard — this ambiguity is not an academic concern but a practical data integrity failure.

The `generated_utc` field in the proposed format's document-level metadata explicitly signals that the response was produced with UTC-aware processing, providing consumers with a verifiable indicator of timezone correctness. This is a property that neither SDMX period notation nor PX-Web local-time labels can offer for sub-daily data.

### 4.5 Relation to existing standards

The proposed format is not intended to replace SDMX, JSON-stat, or PX-Web. It is designed as an additional dissemination layer for:

- Public developer APIs
- AI agent tool integrations
- Lightweight dashboard and visualization applications
- LLM context window injection

Institutional exchange, metadata governance, revision tracking, and multidimensional reporting should continue to use appropriate existing standards. The proposed format addresses a gap in the dissemination stack, not a deficiency in the institutional exchange stack.

---

## 5. Comparative Assessment

### 5.1 Structural comparison

Table 1 summarizes key structural characteristics of the formats discussed.

**Table 1. Structural comparison of statistical dissemination formats**

| Dimension | SDMX-ML | SDMX-JSON | JSON-stat | PX-Web JSON | Proposed format |
|---|---|---|---|---|---|
| Serialization | XML | JSON | JSON | JSON | JSON |
| Time representation | Period strings (non-ISO) | Period strings (non-ISO) | Period strings | Period strings | Unix timestamps (ms) |
| Time labels ISO 8601 compliant | No | No | Partial | No | Yes (UTC integers) |
| External schema resolution | Required | Required | Partial | Partial | Not required |
| Response self-contained | No | No | Partial | Partial | Yes |
| Unique series identifier | Dataflow + key | Dataflow + key | Dimension value | Table + variable name | Single `name` field |
| Nesting depth (typical) | 6–8 | 4–6 | 3–4 | 3–4 | 2 |
| Multidimensional support | Full | Full | Full | Full | Single series |
| Embedded unit/source | Partial | Partial | No | No | Yes |
| Missing value handling | Coded | Coded | `null` | `null` | `null` |
| Designed for human navigation | Partial | No | No | Yes | No |
| Institutional interoperability | Full | Full | Good | Regional | Minimal |
| AI token efficiency | Low | Medium | Medium | Medium | High |

### 5.2 Payload size illustration

To illustrate the token and payload implications, consider a single monthly time series with 60 observations (5 years of monthly data). The following are approximate character counts for the same data in each format:

- **SDMX-ML**: ~8,000–15,000 characters (depending on DSD verbosity)
- **SDMX-JSON**: ~4,000–7,000 characters
- **JSON-stat**: ~2,500–4,000 characters
- **PX-Web JSON**: ~2,000–3,500 characters
- **Proposed format**: ~1,200–1,800 characters

For LLM-based pipelines, where context window tokens are a binding constraint and each token represents approximately 4 characters, the proposed format requires roughly 300–450 tokens for a 60-observation series, compared to 2,000–3,750 for SDMX-ML. At scale across multiple series, this difference is material for both cost and feasibility.

*Note: These are indicative estimates. Empirical benchmarking across specific NSO products is a direction for future work.*

### 5.3 Parsing steps comparison

Table 2 illustrates the number of distinct parsing or resolution steps required to extract a time-value sequence from each format.

**Table 2. Steps required to extract a time-value sequence**

| Format | Steps required |
|---|---|
| SDMX-ML | (1) Retrieve DSD, (2) resolve code lists, (3) map dimension codes, (4) parse time periods, (5) extract values |
| SDMX-JSON | (1) Retrieve DSD, (2) resolve code lists, (3) parse time periods, (4) extract values |
| JSON-stat | (1) Identify time dimension, (2) match value vector indices, (3) parse time labels, (4) extract values |
| PX-Web JSON | (1) Discover product-specific dimension variable names, (2) parse dimension codes, (3) parse time labels, (4) extract values |
| Proposed format | (1) Read `data` array |

The proposed format achieves the minimum possible parsing path for time series extraction.

### 5.4 Practical developer and AI-agent assessment

Table 3 provides a practitioner-oriented summary that extends the structural comparison to include the World Bank API — the most widely adopted simple REST data source — as a real-world benchmark, and evaluates the formats across dimensions that are directly relevant to AI-agent and application developer workflows.

**Table 3. Practical assessment for developer and AI-agent consumption**

| Dimension | SDMX | PX-Web / JSON-stat | World Bank API | Proposed format |
|---|---|---|---|---|
| Format | XML primary, JSON-SDMX optional | JSON-stat or .px text | JSON / XML | Plain JSON |
| Data + metadata | Separate — structure message + data message | Separate — requires dimension lookup | Semi-inline, nested | Inline — one file, self-contained |
| Time series shape | Complex cube | Flat array + dimension index math | Array of `{date, value}` objects | `[[epoch_ms, value], …]` — minimal |
| Human readability (raw) | Low — verbose XML/URNs | Low — codes, no labels inline | Medium | High — description, unit, source inline |
| LLM parsability | Very hard — schema required | Hard — dimensional pivot | Medium | Easy — no pivot, no external lookup |
| Timestamp format | Period strings (non-ISO) | Period strings | String (`"2024"`, `"2024Q1"`) | UTC epoch ms — sortable, unambiguous |
| Frequency field | Implicit in structure | Implicit in dimension | Implicit in indicator metadata | Explicit `frequency` field in document |
| Chart-ready output | No | No | No | Yes — `style{}` sub-object included |
| Multi-series in one file | Yes (complex) | Yes (cube) | No — one indicator per call | Yes — clean `series[]` array |
| Series discovery | Requires dataflow/structure first | Requires catalog API first | Requires indicator search first | URL-based, direct |
| AI/LLM token efficiency | Low | Medium | Medium | High |
| Institutional interoperability | Full | Good / Regional | Good | Minimal |

The World Bank API is useful as a benchmark precisely because it represents the upper bound of usability among established institutional sources: simple REST, consistent JSON, no schema resolution. Yet it still uses string-based time labels, returns one indicator per call, and provides no rendering metadata. The proposed format improves on all three dimensions while remaining equally accessible to consume.

---

## 6. Discussion

### 6.1 The dissemination stack model

The principal policy implication of this analysis is that NSOs should consider adopting a layered dissemination model, in which different format layers serve different consumer classes:

- **Institutional exchange layer**: SDMX (governance, provenance, multidimensionality)
- **Developer/application layer**: The proposed lightweight JSON (or similar)
- **Visualization layer**: NSO-maintained chart interfaces
- **Bulk download layer**: CSV, Parquet, or similar

This model is not novel — NSOs already maintain multiple channels — but it requires explicit recognition that AI agent consumption is a distinct consumer class with distinct format requirements.

### 6.2 Provenance and metadata integrity

A legitimate concern with simplified formats is the potential for metadata loss. A compact format that omits revision history, confidentiality classifications, and methodology references could mislead users who do not consult primary documentation. The proposed format does not attempt to replace SDMX's provenance model.

Producers adopting the proposed format should:
- Include a `source` field with the producing institution name
- Include a `last_updated` field
- Include a reference URL linking to full methodology documentation
- Clearly document any pre-applied transformations in the `transform` field

Responsibility for communicating data limitations and revision policies remains with the producing institution.

### 6.3 Standardization and adoption

The proposed format would benefit from community-level standardization of field names, controlled vocabularies for `freq` and `unit`, and naming conventions for the `name` field. This could be achieved through a profile agreement among interested NSOs, analogous to the JSON-stat community process (Badosa, 2012).

Formal standardization through SDMX or ISO bodies would provide stronger interoperability guarantees but would likely require a longer adoption timeline than the pace of AI system evolution warrants. A pragmatic path would be to define a minimal open specification with an open-source reference implementation, allowing early adoption while formal standardization is pursued in parallel.

### 6.4 Limitations

This paper presents a conceptual and analytical case for the proposed format. Several important areas require further empirical investigation:

- **Parsing performance benchmarks**: Formal measurement of parsing time and token cost across formats for standardized test series.
- **AI agent behavior studies**: Empirical study of data source selection behavior in LLM-based analytical systems.
- **NSO implementation feasibility**: Assessment of implementation cost for NSOs with existing SDMX or JSON-stat infrastructure.
- **User studies**: Assessment of developer experience with the proposed format versus alternatives.

Future work should address these gaps to provide a more complete empirical foundation for format design recommendations.

### 6.5 The urgency of action

The AI transition in data consumption is occurring faster than the typical standards development cycle. NSOs that delay lightweight format adoption risk a sustained selection effect in which AI-driven workflows increasingly rely on lower-quality aggregated sources that already provide simple APIs. The marginal cost of adding a lightweight JSON endpoint to an existing SDMX API is low; the cost of sustained marginalization of official statistics in AI ecosystems is substantially higher.

### 6.6 MCP servers and the limits of interface solutions

A growing number of institutions are exploring Model Context Protocol (MCP) servers as a mechanism for making existing data and APIs accessible to AI agents. MCP provides a standardised interface layer through which an AI system can discover and invoke data sources, reducing the need for custom integrations at the agent level. The approach is technically sound and can improve the discoverability and usability of data from a consumer perspective.

However, MCP servers risk being treated as a substitute for sound underlying data organisation rather than a complement to it. An institution whose data is stored in non-standard datetime formats, inconsistent unit encodings, or frequency-specific period notation does not resolve these problems by placing an MCP layer on top. The MCP interface can translate queries but cannot correct structural deficiencies in the data it exposes: if the underlying series lacks unambiguous time encoding, the MCP server must implement conversion logic that reintroduces the same parsing complexity — timezone handling, period-code resolution, cross-frequency alignment — that the interface layer was intended to eliminate.

MCP also introduces infrastructure and maintenance obligations. A server must be deployed, versioned, and kept consistent with the data sources it exposes. Users and AI systems must discover and authenticate against it; discoverability is not automatic. For organisations with limited technical capacity, MCP deployment may represent a higher implementation cost than restructuring data publication to a well-designed open format that any HTTP client can consume directly without prior configuration.

The practical implication is that MCP is most effective when the underlying data is already well-structured: unambiguous timestamps, consistent identifiers, self-contained metadata. In that case, MCP adds genuine value by automating discovery and reducing client-side integration work. When the data is structurally deficient, MCP defers rather than resolves the underlying problem. The design investment argued for throughout this paper — correct time encoding, minimal mandatory structure, embedded metadata — is not an alternative to MCP; it is a prerequisite for MCP to function well.

### 6.7 Adoption pathways for smaller producers

The arguments in this paper are not limited to large NSOs with established SDMX infrastructure. For many statistical producers — regional agencies, national offices in smaller or lower-income countries, research institutions, and sector-specific data providers — the dissemination choice is not between a lightweight JSON layer and a full institutional exchange platform. For a substantial portion of statistical producers globally, it is between a lightweight JSON format and continued reliance on PDF reports.

PDF remains the predominant dissemination format across much of Africa and Asia. A machine-readable JSON file produced from the same database that generates the PDF report, and served over a standard HTTP endpoint, represents a transformative improvement in accessibility at low incremental cost. Such a file can be generated in any programming language, hosted on any web server, and consumed directly by charting libraries, analytical tools, and AI agents without custom integration work.

A recurring failure mode in dissemination practice is format degradation: organisations that hold data internally in a correctly structured form — with timestamps, consistent identifiers, and clean unit labels — often destroy this structure at the point of publication, collapsing timestamps to period strings, converting UTC to local time without timezone annotation, or embedding values in formatted tables that are designed for human reading. The cost of this transformation is borne entirely by downstream consumers, who must reverse-engineer the original structure before the data can be used programmatically. The design principle implied by this observation is simple: publish data in the form in which it already exists internally. If the internal system stores UTC timestamps, publish UTC timestamps. If series carry unique identifiers in the production database, preserve those identifiers in the public output. Minimising format transformation at publication reduces maintenance burden, preserves data integrity, and eliminates a class of errors that arises from imperfect conversion.

Applications capable of querying a time-series database and producing a well-structured JSON response can be built and maintained in any modern programming language. The marginal effort of generating this output is low for any producer that already holds data in an organised, datetime-correct internal format; the marginal benefit for data consumers — human and AI alike — is substantial.

A working demonstration of this approach is available at Harmonize.no (Soberg, 2026), where a set of official-style time series — consumer prices, producer prices, and energy data — are published as plain JSON files and rendered directly as interactive charts, tabular reports, and downloadable CSV files using browser-based templates. The full dissemination stack is maintained by a single developer without enterprise infrastructure, illustrating that the proposed format requires no specialised tooling to implement or operate.

---

## 7. Conclusion

This paper has argued that the emergence of AI agents and LLM-based analytical systems as significant data consumers creates a structural tension with existing statistical dissemination formats. SDMX, JSON-stat, and PX-Web — designed for institutional clients and human-navigable interfaces — impose parsing complexity and payload overhead that AI systems are not equipped to handle efficiently, producing a selection effect that disadvantages official statistics relative to simpler alternatives.

A compact JSON representation built on Unix timestamps, minimal mandatory structure, and embedded metadata offers a practical path toward making official time series more accessible in AI-era environments. The format is designed as a complementary dissemination layer, not a replacement for institutional standards, and can be implemented incrementally alongside existing infrastructure.

The central design insight is simple but consequential: when time is represented as an unambiguous integer, when metadata travels with data, and when structure is minimized to what is essential, AI systems — and developers, and lightweight applications — can consume official statistics with the same ease they currently apply to simpler but less authoritative sources. The question is not whether to maintain SDMX, but whether to recognize AI agents as a distinct consumer class that official statistics must serve.

This paper argues, finally, for a broader shift in design philosophy. The dissemination systems that dominate official statistics today were built around a foundational assumption: that a human being is the terminal consumer, navigating menus, selecting table dimensions, and reading output on a screen. That assumption shaped every architectural decision — the interactive query builders of PX-Web, the DSD-based resolution model of SDMX, the table-oriented structure of JSON-stat. Each system is well-designed for the consumer it was built for.

The emerging requirement is to invert this priority. Data should be structured for machine and AI consumption first, with human-facing interfaces constructed as a layer above that foundation — not the other way around. This inversion is not merely a preference; it is a practical necessity. A machine-first data layer, with stable unique identifiers, unambiguous timestamps, and self-contained responses, can serve both AI agents and human users: visualization interfaces, reports, and download tools can all be generated from the same underlying format. The reverse does not hold. A dissemination architecture designed around human navigation cannot be made machine-friendly through API wrappers alone, as the experience of adapting PX-Web and SDMX endpoints for programmatic use has repeatedly demonstrated.

Statistical offices that adopt machine-first design principles will be positioned to serve the full spectrum of modern data consumers — from AI agents performing automated retrieval to analysts interpreting results in natural language interfaces. Those that retain human-navigation assumptions as the primary organizing principle of their dissemination architecture risk progressive marginalization in the data ecosystems where their authority and quality are most needed.

A final observation concerns the temporal trajectory of official statistics itself. The frequency of statistical publication is increasing across domains: energy consumption, transport flows, price monitoring, and financial indicators are moving from monthly to weekly, daily, and hourly cycles, driven by the availability of administrative and sensor data sources at sub-daily granularity. Norway's electricity statistics are a concrete example: source data arrives at hourly resolution, with daylight saving time transitions producing two distinct observations at the local clock time 02:00 — one in summer time, one in winter time — that must be correctly distinguished to preserve data integrity. This is not an edge case but a routine property of any sub-daily series anchored to a local timezone. As more NSOs move toward near-real-time dissemination, the capacity to store observations in UTC and convert to local time with full DST correctness at the query layer becomes a core infrastructure requirement, not a specialised capability. Formats designed around monthly or quarterly period codes carry no architectural path toward this requirement; a UTC timestamp-based format requires no revision to accommodate it. The design choices argued for in this paper are therefore not only appropriate for the current generation of statistical dissemination needs, but structurally aligned with the higher-frequency, timezone-aware statistical infrastructure that the next decade will require.

---

## References

Anthropic (2024). *Claude: A Next-Generation AI Assistant*. Technical Report. San Francisco: Anthropic.

Badosa, X. (2012). *JSON-stat: A Simple Lightweight JSON Format for Statistical Data*. Retrieved from https://json-stat.org

BIS, ECB, Eurostat, IMF, OECD, UN, World Bank (2005). *SDMX Technical Standards, Version 1.0*. SDMX Secretariat.

Brown, T., Mann, B., Ryder, N., Subbiah, M., Kaplan, J., Dhariwal, P., ... & Amodei, D. (2020). Language models are few-shot learners. *Advances in Neural Information Processing Systems*, 33, 1877–1901.

Eurostat (2020). *SDMX Information Model: Version 3.0*. Publications Office of the European Union, Luxembourg.

IEEE (2017). *IEEE Std 1003.1-2017 (POSIX.1-2017): Standard for Information Technology — Portable Operating System Interface (POSIX)*. IEEE.

ISO (2019). *ISO 8601-1:2019: Date and Time — Representations for Information Interchange*. International Organization for Standardization.

OpenAI (2023). *GPT-4 Technical Report*. arXiv:2303.08774. Retrieved from https://arxiv.org/abs/2303.08774

Schick, T., Dwivedi-Yu, J., Dessì, R., Raileanu, R., Lomeli, M., Zettlemoyer, L., ... & Scialom, T. (2023). Toolformer: Language models can teach themselves to use tools. *Advances in Neural Information Processing Systems*, 36.

SDMX Global Conference (2021). *SDMX 3.0: What's New*. SDMX Secretariat. Retrieved from https://sdmx.org

Soberg, E. (2026). *Harmonize: A lightweight time-series dissemination platform*. Retrieved from https://harmonize.no

Statistics Sweden (2020). *PX-Web API Documentation, Version 2019 v1*. SCB Technical Documentation Series. Stockholm: Statistics Sweden.

Sundgren, B. (2007). What is a statistical metadata system? *Proceedings of the 56th Session of the International Statistical Institute*. Lisbon.

Wei, J., Wang, X., Schuurmans, D., Bosma, M., Ichter, B., Xia, F., ... & Zhou, D. (2022). Chain-of-thought prompting elicits reasoning in large language models. *Advances in Neural Information Processing Systems*, 35, 24824–24837.

---

*Submitted to the Journal of Official Statistics*
*Word count (excluding abstract, tables, references): approx. 4,200*

---

## Appendix A: Minimal Conformant Response Example

The following is a minimal conformant response using only mandatory fields:

```json
{
  "series": [
    {
      "name": "GDP:VOLUME",
      "unit": "MEUR",
      "data": [
        [1640995200000, 12540.2],
        [1648771200000, 12688.5],
        [1656633600000, 12812.1],
        [1664582400000, 12755.8]
      ]
    }
  ]
}
```

This response is directly usable by any JSON-capable client without additional parsing steps.
