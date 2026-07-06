# An AI-Friendly JSON Representation for Official Time Series: A Practical Alternative for Statistical Dissemination

---

**Erik Søberg**
Statistics Norway (Statistisk sentralbyrå), Oslo, Norway
erik.soberg@ssb.no

---

## Abstract

The increasing adoption of large language models (LLMs) and AI agents in statistical analysis is reshaping the requirements placed on dissemination systems of national statistical offices (NSOs). Current institutional formats — notably SDMX, JSON-stat, and PX-Web — were designed for institutional interoperability and multidimensional reporting, not for machine-native, low-overhead consumption. A central limitation shared across these formats is the absence of a universal datetime standard: frequency-specific period notations such as `2025-Q1`, `2025M12`, and `2025W53` are neither directly sortable nor directly comparable across series without conversion steps that impose implementation complexity and error risk. PX-Web, in particular, was designed as a human navigation interface, and its underlying PC-Axis storage model reflects disk-space constraints of a computing era now more than three decades past — not the retrieval performance and direct integration requirements of modern analytical systems. This paper proposes a simplified JSON representation for statistical time series, based on epoch timestamps (milliseconds elapsed since 1970-01-01 00:00:00 UTC), minimal structure, and embedded metadata. We argue that a complementary lightweight machine-oriented format can reduce these barriers and make official statistics more accessible in AI-era environments without replacing existing institutional standards. We further argue that interface technologies such as Model Context Protocol (MCP) servers, while valuable for AI discoverability, cannot substitute for well-structured underlying data: MCP is most effective when the data it exposes already uses unambiguous time encoding and self-contained metadata — conditions that the proposed format is designed to satisfy.

**Keywords:** statistical dissemination, artificial intelligence, time series, JSON, SDMX, epoch timestamps, MCP, interoperability

---

## 1. Introduction

The role of official statistics in policy, research, and public discourse has never been more significant, yet the technical infrastructure for statistical dissemination has not kept pace with the rapid evolution of data consumption practices. A new class of consumers has emerged: AI systems, large language models (LLMs), and automated data pipelines that retrieve, interpret, and integrate data at scale. These systems operate under fundamentally different constraints than human users or traditional institutional clients.

Where human users benefit from structured navigation interfaces and institutional conventions, AI systems favor formats that minimize parsing steps, reduce structural ambiguity, and support direct integration into computation. This shift raises a practical question for statistical offices: should dissemination formats evolve to serve these new consumers, and if so, how?

This paper argues that existing formats such as SDMX and PX-Web, while robust for their intended purposes, present obstacles for AI-era consumption. We propose a compact JSON representation as a complementary dissemination layer, not a replacement for institutional standards, and discuss its design principles, trade-offs, and practical implications.

---

## 2. The Changing Landscape of Statistical Data Consumption

### 2.1 AI systems as data consumers

Large language models and AI agents are increasingly being used by analysts, researchers, and developers to retrieve and analyze statistical data. Systems such as GPT-4, Claude, and similar tools can be instructed to retrieve data via APIs and web services, reason over retrieved observations, and present findings in natural language (Brown et al., 2020; OpenAI, 2023).

In practice, the behavior of AI agents when selecting data sources is strongly influenced by the complexity of the access mechanism. Agents operating with tool-use capabilities tend to favor APIs that return predictable, flat, immediately usable structures. When faced with deeply nested schemas or format-specific parsing requirements, AI systems incur additional reasoning steps, increasing both computational cost (in terms of tokens and latency) and the risk of misinterpretation.

This creates a practical selection effect: AI agents and developers tend to gravitate toward sources such as the World Bank API or FRED (Federal Reserve Economic Data), whose simple REST endpoints return compact JSON, even when more authoritative national sources would better serve the analytical need.

### 2.2 The cost of complexity for official statistics

National statistical offices produce data of superior quality: higher frequency, more granular geographic coverage, and better methodological documentation than many international aggregators. Yet this quality advantage is often inaccessible in AI-driven workflows because the dissemination layer imposes complexity that deters automated consumption.

This is not merely a technical inconvenience. If AI systems systematically bypass official statistics in favor of simpler but less authoritative sources, the implications for evidence-based policymaking and research integrity are significant. The design of dissemination formats thus carries consequences beyond technical efficiency.

---

## 3. Limitations of Current Formats

### 3.1 SDMX

The Statistical Data and Metadata eXchange standard (SDMX Global Conference, 2021; BIS et al., 2005) is the principal interoperability framework adopted by Eurostat, the IMF, the OECD, the ECB, and most major NSOs. It provides a rigorous model for multidimensional statistical data, including concept schemes, code lists, data structure definitions (DSDs), and provenance tracking.

While SDMX is exceptionally powerful for institutional exchange and governance, it presents several obstacles for lightweight consumption:

- **Schema resolution**: Consuming SDMX data correctly requires resolving external references (DSDs, code lists) before the data itself can be interpreted.
- **Hierarchical nesting**: SDMX-ML documents are deeply nested, requiring specialized parsers or libraries that may not be available in all development environments.
- **Non-standard time period encoding**: SDMX uses proprietary frequency-specific period notation (`2025-Q1`, `2025W53`, `2025M12`) that does not conform to ISO 8601 and requires calendar-aware parsing logic for correct ordering and alignment. Converting these strings to sortable datetime values demands additional processing at every point of consumption — and the conversion rules differ across frequencies, introducing meaningful implementation complexity and a recurring source of errors when series of different frequencies are compared.

These features are appropriate — even essential — in institutional contexts. They become obstacles in lightweight, AI-native workflows.

### 3.2 JSON-stat

JSON-stat (Badosa, 2012) was developed as a compact JSON format for statistical tables, and is used by Statistics Norway and several other NSOs via their API services. It reduces the verbosity of SDMX-ML while retaining a tabular, dimension-based structure.

However, JSON-stat is optimized for tabular cross-sectional data rather than time series retrieval or charting. Its nested dimension array structure still requires several parsing steps before a simple time-value sequence can be extracted. It does not natively support Unix epoch timestamps, requiring string-based time identifiers that inherit the same frequency-specific ambiguity as SDMX.

A practical consequence of these format limitations is visible in the quality of visualization offered by NSO websites. Interactive, zoomable charts displaying multiple series at different frequencies remain rare on official statistical portals in 2026, despite being standard practice in financial data services and commercial platforms for years. The pattern that emerges is revealing: third-party providers increasingly download data from official statistical databases, store it in time-aware databases with proper datetime support, and present it in richer, more accessible visualizations than the originating NSO. Official data thus reaches users through intermediaries who have solved the format and datetime problems that the original dissemination layer left unresolved.

### 3.3 PX-Web

PX-Web, developed by Statistics Sweden and widely deployed across Nordic and other NSOs, provides an API layer over PC-Axis format databases (Statistics Sweden, 2020). It is important to understand what PX-Web was designed to be: primarily a user interface for human navigation of statistical tables. Its core interaction model — selecting dimensions, filtering variable values, and drilling down through hierarchical menus — reflects the assumption that a person is operating the system. The API was added as a secondary capability on top of this human-oriented architecture, not designed from the ground up for programmatic or machine consumption.

This origin has lasting consequences. The underlying PC-Axis file format was designed more than three decades ago, at a time when disk space was a binding constraint on data storage architecture. The format's flat, text-based structure with positional value encoding reflects optimization for compact storage rather than for retrieval performance or computational efficiency. Modern systems — which prioritize query speed, columnar access patterns, and direct integration with analytical pipelines — are poorly served by a storage model inherited from a different era of computing constraints.

A further structural problem is the absence of a consistent series identifier across PX-Web products. In PX-Web, retrieving a series requires knowing not only the table it belongs to, but also the names of the dimension variables used within that specific table — names that are defined independently by each statistical product and each NSO. A consumer querying consumer price data from one table might need to filter on a variable called `contents_code`, while the equivalent variable in another table is named `consumer-groups`, and in a third it is `product_category`. There is no universal series key that works across tables or across NSOs.

This contrasts directly with the approach taken by simpler data services. The World Bank API identifies every series by a single, globally unique indicator code (e.g., `FP.CPI.TOTL`). The proposed format uses a single `name` field as a stable, unique identifier for each series, independent of any table or dimension context. For an AI agent or developer building a data pipeline, this difference is fundamental: a unique name can be stored, shared, and reused without requiring any knowledge of the source system's internal dimension structure.

Like SDMX, PX-Web uses string-based time labels that are not standardized across statistical products or across NSOs. A consumer receiving PX-Web responses for series from different national offices may encounter time labels in formats such as `"2025M01"`, `"2025-01"`, `"January 2025"`, or `"2025Q1"` — all representing the same period but requiring different parsing logic. This absence of a common datetime standard means that every integration project must implement its own time-label parsing and conversion layer, a cost that accumulates across every developer, every AI system, and every data pipeline that attempts to work with official statistics.

### 3.4 The shared date-time problem

The absence of a universal, machine-native date-time standard is the single most consequential shared limitation of both SDMX and PX-Web for AI-era consumption. Neither format adopts ISO 8601 consistently, and neither supports numeric timestamps that are directly sortable and comparable without parsing.

The practical consequence is that any system consuming statistical time series from these formats must implement conversion logic for every frequency type encountered: annual, quarterly, monthly, weekly, and daily data each require separate handling. When comparing series across frequencies — for example, aligning monthly price indices with quarterly GDP estimates — the consumer must additionally resolve reference period conventions (is a monthly observation dated at the start, middle, or end of the month?) that are often implicit rather than explicitly stated in metadata.

For AI agents operating under token and latency constraints, this conversion overhead is not merely inconvenient — it is a structural deterrent. An agent that encounters ambiguous time labels must reason about calendar conventions rather than proceeding directly to analysis, increasing both error risk and computational cost. The result is a systematic pressure toward data sources that provide unambiguous, ready-to-use time values, regardless of whether those sources are authoritative.

## 4. A Proposed Lightweight JSON Representation

We propose a compact JSON format for statistical time series designed for direct consumption by AI systems and modern application pipelines.

### 4.1 Core structure

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

### 4.2 Design principles

**Machine-native time encoding.** Unix timestamps (milliseconds since 1970-01-01 00:00:00 UTC) are unambiguous, sortable integers. They require no calendar interpretation, no frequency-specific parsing rules, and no resolution of institutional conventions. They are natively supported in JavaScript, Python, R, and virtually all modern computing environments.

A further consequence of Unix timestamp encoding is unambiguous timezone handling. The proposed format stores all observations in UTC. Where local-time output is required — for example, displaying Norwegian energy consumption in Central European Time — timezone conversion is applied at the query layer, using the full IANA timezone database supported by modern database engines. This approach correctly handles daylight saving time transitions: on the autumn clock-change night, the hour from 02:00 to 03:00 occurs twice in local time. A UTC-based system produces two distinct observations at the correct UTC epochs, preserving both readings. Period-notation formats that store local time labels cannot represent this distinction — both observations would carry the label `02:00`, with no way to determine which belongs to summer time and which to winter time. For sub-daily statistical series — energy consumption, transport flows, meteorological observations — DST-correct representation is a data quality requirement, not an edge case.

A structural consequence of this design choice is that the format imposes no frequency constraint. Because observation time is an explicit timestamp rather than a derived period code, a single file can contain series of different frequencies — a monthly price index alongside a quarterly national accounts aggregate — without any special alignment logic. Mixed-frequency comparisons, which require non-trivial conversion steps in SDMX and PX-Web, become a natural property of the format. As a further consequence, the same representation handles variable-frequency and irregular observation schedules without modification: meteorological forecast data that transitions from hourly to three-hourly to twelve-hourly intervals as the forecast horizon extends, or sensor and measurement data with no fixed periodicity, are stored identically to regular statistical series. The `frequency` field, where present, is informational metadata for the consumer — the timestamps themselves are always the authoritative time reference.

**Minimal mandatory structure.** The format defines a small set of required fields (`name`, `unit`, `data`) and a larger set of optional fields (`title`, `source`, `freq`, `base_year`, `last_updated`). This allows producers to add richness incrementally without breaking consumers that expect only the minimal structure.

**Tuple-based data storage.** Each observation is a two-element array `[timestamp, value]`, which is the most compact and computationally natural representation for ordered time-value pairs. It imposes no parsing overhead and is directly consumable by charting libraries, pandas, R time series objects, and LLM context windows.

**Embedded metadata.** All metadata necessary to interpret the series is co-located with the data. This eliminates the need for separate schema resolution steps and ensures that a single API response is self-contained.

### 4.3 Optional visualization fields

For applications that consume data directly for display, the format supports optional rendering hints:

```json
{
  "type": "spline",
  "color": "#628DCB",
  "y_axis": 0,
  "transform": "pct12",
  "label": "CPI (12-month change)"
}
```

These fields are ignored by consumers that do not recognize them, preserving backward compatibility.

### 4.4 Relation to existing standards

The proposed format is designed as a *dissemination layer complement*, not a replacement for SDMX or JSON-stat. It is appropriate for:

- Public API endpoints targeting developers and AI systems
- Lightweight dashboards and visualization applications
- LLM tool integrations and automated pipelines

Institutional exchange, metadata governance, and multidimensional reporting should continue to use SDMX or appropriate standards. The two layers serve different consumers and are not mutually exclusive.

---

## 5. Comparative Assessment

The table below summarizes the key characteristics of the formats discussed, including the World Bank API as a widely used reference point for simple REST-based data access.

| Dimension | SDMX | PX-Web / JSON-stat | World Bank API | Proposed format |
|---|---|---|---|---|
| Format | XML primary, JSON-SDMX optional | JSON-stat or .px text | JSON / XML | Plain JSON |
| Data + metadata | Separate — structure message + data message | Separate — requires dimension lookup | Semi-inline, nested | Inline — one file, self-contained |
| Time representation | Period strings (non-ISO) | Period strings | String (`"2024"`, `"2024Q1"`) | Unix timestamps (ms, UTC) |
| Time series shape | Complex cube | Flat array + dimension index math | Array of `{date, value}` objects | `[[epoch_ms, value], …]` — minimal |
| Schema resolution required | Yes | Partial | No | No |
| Unique series identifier | Dataflow + key | Table + variable name | Single indicator code | Single `name` field |
| Human readability (raw) | Low | Low | Medium | High — description, unit, source inline |
| LLM parsability | Very hard — schema required | Hard — dimensional pivot | Medium | Easy — no pivot, no external lookup |
| Chart-ready output | No | No | No | Yes — `style{}` sub-object included |
| Multi-series in one file | Yes (complex) | Yes (cube) | No — one indicator per call | Yes — clean `series[]` array |
| AI/LLM token efficiency | Low | Medium | Medium | High |
| Institutional interoperability | Full | Good / Regional | Good | Minimal |

The World Bank API is instructive as a benchmark: it is the most AI-accessible of the established sources, yet it still uses string-based time labels, returns one indicator per call, and provides no chart rendering hints. The proposed format improves on all three dimensions while remaining equally simple to consume.

The proposed format trades multidimensional support and institutional interoperability for simplicity, self-containment, and AI-native efficiency. This is the appropriate trade-off for a lightweight dissemination layer.

---

## 6. Discussion

### 6.1 Complementarity, not replacement

A common objection to simplified formats is that they fragment the statistical ecosystem. This concern is valid but overstated if the simplified format is positioned as an additional layer rather than a replacement. NSOs already maintain multiple dissemination channels (API, bulk download, visualization interface, institutional exchange). A lightweight JSON endpoint requires no dismantling of SDMX infrastructure.

### 6.2 The provenance obligation

A legitimate concern is metadata loss. A compact format that omits methodology notes, revision history, and confidentiality classifications could mislead users. The proposed format addresses this minimally through the `source` and `last_updated` fields, but does not attempt to replace SDMX's provenance model. Producer documentation, terms-of-use links, and full methodology remain the responsibility of the institutional dissemination layer. Additional fields — such as language tags or documentation URLs — can be included as optional extensions. However, experience consistently shows that maintaining a large number of metadata fields carries a cost that is frequently underestimated: fields that are difficult to populate are left incomplete or inconsistent across products. A small set of well-maintained, complete metadata fields is more useful in practice than a comprehensive schema in which most fields are only partially populated.

### 6.3 Standardization prospects

The proposed format would benefit from adoption of a common naming convention for the `name` field and a controlled vocabulary for `freq` and `unit`. This could be achieved through a lightweight profile agreement among interested NSOs, analogous to the JSON-stat community process (Badosa, 2012), without requiring a full new standard. A standardized format of this kind has a further practical benefit: it enables NSOs to build reusable web templates for charts, data downloads, and statistical articles that consume the same format without custom integration work for each product. The same JSON response that feeds an interactive chart can feed a data download link or a machine-readable API — eliminating the duplication that currently exists between visualization, dissemination, and exchange layers. Standardization of unit notation is a necessary part of this effort; a value reported as `KWh` in one product and `kW/h` in another cannot be treated as the same unit by automated systems, and such inconsistencies undermine the interoperability that a common format is intended to provide.

### 6.4 MCP servers and the limits of interface solutions

A growing number of institutions are considering Model Context Protocol (MCP) servers as a practical path to making existing data and APIs accessible to AI systems. MCP provides a structured discovery layer that can meaningfully improve usability for AI agents — but it does not replace the need to organise data correctly in the first place. An institution whose underlying series use non-standard datetime formats, inconsistent unit encodings, or frequency-specific period codes cannot resolve these structural problems by adding an MCP interface layer. The conversion logic simply moves from the consumer to the server, and must be maintained there with the same rigour that would otherwise be required of the consumer.

MCP also introduces additional infrastructure to deploy and maintain, and places a discovery requirement on users and AI systems: they must know the server exists and be able to connect to it. This is not a trivial condition, particularly for smaller organisations or producers whose data is not widely indexed. A well-structured, self-contained JSON file published over a standard HTTP endpoint is discoverable by any web client and consumable without prior configuration — a simpler path to the same accessibility goal.

The sequencing principle implied by this analysis is straightforward: invest first in data quality and format correctness, then in interface layers. MCP is most effective when the data it exposes is already structured for machine consumption. When it is not, MCP defers rather than resolves the underlying problem.

### 6.5 Adoption pathways for smaller producers

The arguments in this paper are not limited to large NSOs with established SDMX infrastructure. For many statistical producers — regional agencies, national offices in smaller or lower-income countries, research institutions, and sector-specific data providers — the dissemination choice is not between a lightweight JSON layer and a full institutional exchange platform. For a substantial share of global statistical producers, the realistic alternative is not SDMX or PX-Web but the PDF report.

PDF remains the predominant dissemination format across much of Africa and Asia. A machine-readable JSON file produced from the same database that generates the PDF report, served over a standard HTTP endpoint, is a transformative improvement in data accessibility at low incremental cost. It can be generated in any programming language, hosted on any web server, and consumed directly by charting libraries, analytical tools, and AI agents without custom integration work. For a producer at this starting point, a lightweight JSON layer is not a complement to a larger system — it is the system, and it is sufficient.

A recurring failure mode in practice is format degradation at publication: organisations that hold data internally in a correctly structured form — with precise timestamps, consistent identifiers, clean unit labels — often destroy this structure when preparing data for dissemination, collapsing timestamps to period strings, converting UTC to local time without timezone annotation, or embedding values in formatted tables designed for human reading. The cost is borne entirely by consumers, who must reverse-engineer the original structure before the data can be used programmatically. The corrective design principle is simple: publish data in the form in which it already exists internally. Preserve the time encoding that is already correct. Minimise transformation at publication. The gain in usability for downstream consumers — human and AI alike — is immediate and substantial.

A working demonstration of this approach is available at Harmonize.no (Soberg, 2026), where consumer price, producer price, and energy time series are published as plain JSON files and rendered as interactive charts, tabular reports, and downloadable CSV files using browser-based templates — maintained by a single developer, without enterprise infrastructure.

---

## 7. Conclusion

Official statistical offices face a structural challenge: their data is authoritative, but their dissemination formats are optimized for institutional clients rather than for the AI systems and developers who increasingly drive data discovery and consumption. The result is a selection effect that systematically favors simpler, less authoritative sources in AI-driven workflows.

A complementary lightweight JSON representation — built on Unix timestamps, minimal structure, and embedded metadata — offers a practical path toward making official statistics more accessible in AI-era environments. Such a format requires no replacement of existing standards and imposes low implementation cost on producers.

The challenge for the statistical community is to recognize that the question is not whether to maintain SDMX, but whether to build a parallel lightweight layer that serves the emerging class of AI and developer consumers. The cost of not doing so is a gradual marginalization of official statistics in the data ecosystems where they are most needed.

Underlying this challenge is a more fundamental shift in design philosophy. The dissemination systems that dominate official statistics today — PX-Web, SDMX browsers, interactive table builders — were built around a single assumption: that a human being is sitting at the other end, clicking through menus, selecting dimensions, and reading a table on a screen. That assumption was reasonable when it was made. It is no longer sufficient.

The emerging requirement is to invert the priority: to design data first for machines and AI, and build human interfaces on top of that foundation — not the other way around. When data is structured for machine consumption from the outset, with unambiguous time encoding, stable unique identifiers, and self-contained responses, human-facing interfaces can be generated from the same source with minimal additional effort. The reverse is not true: a system designed for human navigation cannot easily be made machine-friendly after the fact, as decades of API wrappers built over PX-Web and SDMX have demonstrated.

Statistical offices that adopt this inverted design logic — machine-first, human interface as a layer above — will be better positioned to serve both the AI systems that increasingly drive data discovery and the human analysts who interpret the results. Those that do not risk becoming invisible in the data ecosystems where their authority matters most.

The move toward higher-frequency official statistics — from monthly to weekly, daily, and hourly publication cycles already underway in energy, transport, and price monitoring — makes UTC-correct timezone handling a forward-looking infrastructure requirement rather than a niche concern. Norway's electricity statistics, for instance, already receive hourly source data in which daylight saving time transitions produce two observations at the local hour 02:00, requiring DST-aware processing to preserve both readings correctly. As more domains shift toward near-real-time dissemination, this class of problem will become routine in official statistics. Formats designed around monthly period codes will require fundamental revision to serve this emerging class of data; a timestamp-based format requires no such revision. The design choices argued for in this paper are thus not only appropriate for today's dissemination needs but positioned for the statistical infrastructure of the next decade.

---

## Références / References

Badosa, X. (2012). *JSON-stat: A Simple Lightweight JSON Format for Statistical Data*. Retrieved from https://json-stat.org

BIS, ECB, Eurostat, IMF, OECD, UN, World Bank (2005). *SDMX Technical Standards, Version 1.0*. SDMX Secretariat.

Brown, T., et al. (2020). Language models are few-shot learners. *Advances in Neural Information Processing Systems*, 33, 1877–1901.

Eurostat (2020). *SDMX Information Model: Version 3.0*. Publications Office of the European Union.

OpenAI (2023). *GPT-4 Technical Report*. arXiv:2303.08774.

SDMX Global Conference (2021). *SDMX 3.0: What's New*. SDMX Secretariat.

Soberg, E. (2026). *Harmonize: A lightweight time-series dissemination platform*. Retrieved from https://harmonize.no

Statistics Sweden (2020). *PX-Web API Documentation*. SCB Technical Documentation Series.

---

*Submitted to STATECO*
*Word count (excluding abstract, tables, references): approx. 2,700*
