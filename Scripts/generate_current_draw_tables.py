#!/usr/bin/env python3
"""Generate current AscNet draw tables from decoded retail draw response dumps.

This intentionally overwrites the single current server config. It does not create
versioned season bundles; Steam/launcher clients cannot stay on old versions.
"""
from __future__ import annotations

import argparse
import json
import sys
from dataclasses import dataclass
from pathlib import Path
from typing import Any


REPO_ROOT = Path(__file__).resolve().parents[1]
DEFAULT_OUTPUT = REPO_ROOT / "AscNet.GameServer" / "Game" / "DrawManager.GeneratedTables.cs"


@dataclass(frozen=True)
class DrawGroupRow:
    id: int
    tag: int
    order: int
    priority: int
    use_item_id: int
    max_bottom_times: int
    type: int
    banner: str
    default_use_draw_id_dict: dict[int, int]
    optional_draw_id_list: list[int]
    tag_black_list_draw_ids: list[int]
    banner_begin_time: int
    banner_end_time: int
    condition_id: int
    show_predict_type: int
    start_time: int
    end_time: int


@dataclass(frozen=True)
class DrawInfoRow:
    id: int
    group_id: int
    draw_type: int
    use_item_id: int
    use_item_count: int
    base_today_count: int
    base_total_count: int
    bottom_times: int
    max_bottom_times: int
    start_time: int
    end_time: int
    banner: str
    resources: dict[int, str]
    resource_ids: dict[int, int]
    btn_draw_count: list[int]
    purchase_ui_type: list[int]
    purchase_id: list[int]
    ex_purchase_ids: list[int]
    capacity_check_type: int
    group_sub_type: int
    is_trigger_specified: bool
    is_show_shop: bool
    is_show_bubble: bool
    use_ten_draw_on_sale_times: int
    daily_limit_times: int
    activity_limit_times: int
    show_priority: int
    up_goods_id: int


@dataclass(frozen=True)
class DrawAdjustActivityRow:
    target_times: int
    target_id: int
    activity_status: int
    activity_id: int
    start_time: int
    end_time: int
    adjust_times: int
    draw_group_id: int
    target_template_ids: list[int]
    source_template_ids: list[int]
    effect_target_template_ids: list[int]


def load_json_object(path: Path, expected_name: str) -> dict[str, Any]:
    with path.open("r", encoding="utf-8") as handle:
        root = json.load(handle)
    if not isinstance(root, dict):
        raise ValueError(f"{path}: expected {expected_name} root object, got {type(root).__name__}.")
    return root


def require_list(root: dict[str, Any], key: str, source: Path) -> list[Any]:
    value = root.get(key)
    if not isinstance(value, list):
        raise ValueError(f"{source}: expected {key} to be a list, got {type(value).__name__}.")
    return value


def require_record(record: Any, source: Path, index: int, list_name: str) -> dict[str, Any]:
    if not isinstance(record, dict):
        raise ValueError(f"{source}: {list_name}[{index}] must be an object, got {type(record).__name__}.")
    return record


def require_int(record: dict[str, Any], key: str, source: Path, index: int, list_name: str) -> int:
    value = record.get(key)
    if not isinstance(value, int) or isinstance(value, bool):
        raise ValueError(f"{source}: {list_name}[{index}].{key} must be an integer, got {value!r}.")
    return value


def require_bool(record: dict[str, Any], key: str, source: Path, index: int, list_name: str) -> bool:
    value = record.get(key)
    if not isinstance(value, bool):
        raise ValueError(f"{source}: {list_name}[{index}].{key} must be a boolean, got {value!r}.")
    return value


def require_string(record: dict[str, Any], key: str, source: Path, index: int, list_name: str) -> str:
    value = record.get(key)
    if not isinstance(value, str):
        raise ValueError(f"{source}: {list_name}[{index}].{key} must be a string, got {value!r}.")
    return value


def require_int_list(record: dict[str, Any], key: str, source: Path, index: int, list_name: str) -> list[int]:
    value = record.get(key)
    if not isinstance(value, list):
        raise ValueError(f"{source}: {list_name}[{index}].{key} must be an integer list, got {type(value).__name__}.")
    result: list[int] = []
    for item_index, item in enumerate(value):
        if not isinstance(item, int) or isinstance(item, bool):
            raise ValueError(f"{source}: {list_name}[{index}].{key}[{item_index}] must be an integer, got {item!r}.")
        result.append(item)
    return result


def require_int_key_int_dict(record: dict[str, Any], key: str, source: Path, index: int, list_name: str) -> dict[int, int]:
    return require_int_key_dict(record, key, source, index, list_name, value_type=int)


def require_int_key_string_dict(record: dict[str, Any], key: str, source: Path, index: int, list_name: str) -> dict[int, str]:
    return require_int_key_dict(record, key, source, index, list_name, value_type=str)


def require_int_key_dict(
    record: dict[str, Any],
    key: str,
    source: Path,
    index: int,
    list_name: str,
    *,
    value_type: type[int] | type[str],
) -> dict[int, Any]:
    value = record.get(key)
    if not isinstance(value, dict):
        raise ValueError(f"{source}: {list_name}[{index}].{key} must be a map, got {type(value).__name__}.")

    result: dict[int, Any] = {}
    for raw_key, raw_value in value.items():
        try:
            int_key = int(raw_key)
        except ValueError as exception:
            raise ValueError(f"{source}: {list_name}[{index}].{key} key {raw_key!r} must be an integer string.") from exception
        if str(int_key) != raw_key:
            raise ValueError(f"{source}: {list_name}[{index}].{key} key {raw_key!r} must be a canonical integer string.")
        if value_type is int:
            if not isinstance(raw_value, int) or isinstance(raw_value, bool):
                raise ValueError(f"{source}: {list_name}[{index}].{key}[{raw_key!r}] must be an integer, got {raw_value!r}.")
        elif not isinstance(raw_value, str):
            raise ValueError(f"{source}: {list_name}[{index}].{key}[{raw_key!r}] must be a string, got {raw_value!r}.")
        result[int_key] = raw_value
    return dict(sorted(result.items()))


def require_code_zero(root: dict[str, Any], source: Path, response_name: str) -> None:
    code = root.get("Code")
    if code != 0:
        raise ValueError(f"{source}: expected {response_name}.Code == 0, got {code!r}.")


def load_draw_group_response(path: Path) -> tuple[list[DrawGroupRow], list[DrawAdjustActivityRow]]:
    root = load_json_object(path, "DrawGetDrawGroupListResponse")
    require_code_zero(root, path, "DrawGetDrawGroupListResponse")

    groups: list[DrawGroupRow] = []
    seen_group_ids: set[int] = set()
    for index, raw_record in enumerate(require_list(root, "DrawGroupInfoList", path)):
        record = require_record(raw_record, path, index, "DrawGroupInfoList")
        group_id = require_int(record, "Id", path, index, "DrawGroupInfoList")
        if group_id in seen_group_ids:
            raise ValueError(f"{path}: duplicate DrawGroupInfoList Id {group_id}.")
        seen_group_ids.add(group_id)
        groups.append(
            DrawGroupRow(
                id=group_id,
                tag=require_int(record, "Tag", path, index, "DrawGroupInfoList"),
                order=require_int(record, "Order", path, index, "DrawGroupInfoList"),
                priority=require_int(record, "Priority", path, index, "DrawGroupInfoList"),
                use_item_id=require_int(record, "UseItemId", path, index, "DrawGroupInfoList"),
                max_bottom_times=require_int(record, "MaxBottomTimes", path, index, "DrawGroupInfoList"),
                type=require_int(record, "Type", path, index, "DrawGroupInfoList"),
                banner=require_string(record, "Banner", path, index, "DrawGroupInfoList"),
                default_use_draw_id_dict=require_int_key_int_dict(record, "UseDrawIdDict", path, index, "DrawGroupInfoList"),
                optional_draw_id_list=require_int_list(record, "OptionalDrawIdList", path, index, "DrawGroupInfoList"),
                tag_black_list_draw_ids=require_int_list(record, "TagBlackListDrawIds", path, index, "DrawGroupInfoList"),
                banner_begin_time=require_int(record, "BannerBeginTime", path, index, "DrawGroupInfoList"),
                banner_end_time=require_int(record, "BannerEndTime", path, index, "DrawGroupInfoList"),
                condition_id=require_int(record, "ConditionId", path, index, "DrawGroupInfoList"),
                show_predict_type=require_int(record, "ShowPredictType", path, index, "DrawGroupInfoList"),
                start_time=require_int(record, "StartTime", path, index, "DrawGroupInfoList"),
                end_time=require_int(record, "EndTime", path, index, "DrawGroupInfoList"),
            )
        )

    if not groups:
        raise ValueError(f"{path}: DrawGroupInfoList is empty; refusing to generate broken draw tables.")

    adjust_rows: list[DrawAdjustActivityRow] = []
    for index, raw_record in enumerate(require_list(root, "DrawAdjustActivityInfoList", path)):
        record = require_record(raw_record, path, index, "DrawAdjustActivityInfoList")
        adjust_rows.append(
            DrawAdjustActivityRow(
                target_times=require_int(record, "TargetTimes", path, index, "DrawAdjustActivityInfoList"),
                target_id=require_int(record, "TargetId", path, index, "DrawAdjustActivityInfoList"),
                activity_status=require_int(record, "ActivityStatus", path, index, "DrawAdjustActivityInfoList"),
                activity_id=require_int(record, "ActivityId", path, index, "DrawAdjustActivityInfoList"),
                start_time=require_int(record, "StartTime", path, index, "DrawAdjustActivityInfoList"),
                end_time=require_int(record, "EndTime", path, index, "DrawAdjustActivityInfoList"),
                adjust_times=require_int(record, "AdjustTimes", path, index, "DrawAdjustActivityInfoList"),
                draw_group_id=require_int(record, "DrawGroupId", path, index, "DrawAdjustActivityInfoList"),
                target_template_ids=require_int_list(record, "TargetTemplateIds", path, index, "DrawAdjustActivityInfoList"),
                source_template_ids=require_int_list(record, "SourceTemplateIds", path, index, "DrawAdjustActivityInfoList"),
                effect_target_template_ids=require_int_list(record, "EffectTargetTemplateIds", path, index, "DrawAdjustActivityInfoList"),
            )
        )

    return groups, adjust_rows


def load_draw_info_response(path: Path) -> list[DrawInfoRow]:
    root = load_json_object(path, "DrawGetDrawInfoListResponse")
    require_code_zero(root, path, "DrawGetDrawInfoListResponse")

    rows: list[DrawInfoRow] = []
    for index, raw_record in enumerate(require_list(root, "DrawInfoList", path)):
        record = require_record(raw_record, path, index, "DrawInfoList")
        rows.append(
            DrawInfoRow(
                id=require_int(record, "Id", path, index, "DrawInfoList"),
                group_id=require_int(record, "GroupId", path, index, "DrawInfoList"),
                draw_type=require_int(record, "DrawType", path, index, "DrawInfoList"),
                use_item_id=require_int(record, "UseItemId", path, index, "DrawInfoList"),
                use_item_count=require_int(record, "UseItemCount", path, index, "DrawInfoList"),
                base_today_count=require_int(record, "TodayCount", path, index, "DrawInfoList"),
                base_total_count=require_int(record, "TotalCount", path, index, "DrawInfoList"),
                bottom_times=require_int(record, "BottomTimes", path, index, "DrawInfoList"),
                max_bottom_times=require_int(record, "MaxBottomTimes", path, index, "DrawInfoList"),
                start_time=require_int(record, "StartTime", path, index, "DrawInfoList"),
                end_time=require_int(record, "EndTime", path, index, "DrawInfoList"),
                banner=require_string(record, "Banner", path, index, "DrawInfoList"),
                resources=require_int_key_string_dict(record, "Resources", path, index, "DrawInfoList"),
                resource_ids=require_int_key_int_dict(record, "ResourceIds", path, index, "DrawInfoList"),
                btn_draw_count=require_int_list(record, "BtnDrawCount", path, index, "DrawInfoList"),
                purchase_ui_type=require_int_list(record, "PurchaseUiType", path, index, "DrawInfoList"),
                purchase_id=require_int_list(record, "PurchaseId", path, index, "DrawInfoList"),
                ex_purchase_ids=require_int_list(record, "ExPurchaseIds", path, index, "DrawInfoList"),
                capacity_check_type=require_int(record, "CapacityCheckType", path, index, "DrawInfoList"),
                group_sub_type=require_int(record, "GroupSubType", path, index, "DrawInfoList"),
                is_trigger_specified=require_bool(record, "IsTriggerSpecified", path, index, "DrawInfoList"),
                is_show_shop=require_bool(record, "IsShowShop", path, index, "DrawInfoList"),
                is_show_bubble=require_bool(record, "IsShowBubble", path, index, "DrawInfoList"),
                use_ten_draw_on_sale_times=require_int(record, "UseTenDrawOnSaleTimes", path, index, "DrawInfoList"),
                daily_limit_times=require_int(record, "DailyLimitTimes", path, index, "DrawInfoList"),
                activity_limit_times=require_int(record, "ActivityLimitTimes", path, index, "DrawInfoList"),
                show_priority=require_int(record, "ShowPriority", path, index, "DrawInfoList"),
                up_goods_id=require_int(record, "UpGoodsId", path, index, "DrawInfoList"),
            )
        )

    if not rows:
        raise ValueError(f"{path}: DrawInfoList is empty; refusing to generate broken draw tables.")
    return rows


def resolve_input_paths(args: argparse.Namespace) -> tuple[Path, list[Path], str]:
    if args.draw_dump_dir is not None:
        if args.draw_group_list is not None or args.draw_info_list:
            raise ValueError("--draw-dump-dir cannot be combined with --draw-group-list or --draw-info-list.")
        dump_dir = args.draw_dump_dir
        if not dump_dir.is_dir():
            raise ValueError(f"{dump_dir}: expected a draw dump directory.")
        group_candidates = sorted(dump_dir.glob("*-DrawGetDrawGroupListResponse.json"))
        if not group_candidates:
            raise ValueError(f"{dump_dir}: no *-DrawGetDrawGroupListResponse.json files found.")
        group_path = group_candidates[-1]

        info_candidates = sorted(dump_dir.glob("*-DrawGetDrawInfoListResponse.json"))
        if not info_candidates:
            raise ValueError(f"{dump_dir}: no *-DrawGetDrawInfoListResponse.json files found.")

        # DrawInfoList responses carry player-scoped counters. In a capture directory the
        # earliest response for each group is the clean login/open-table snapshot; later
        # responses may include pulls made during the capture. Group list selection state is
        # intentional current static metadata, so the latest group response is still used.
        first_info_path_by_group: dict[int, Path] = {}
        for path in info_candidates:
            rows = load_draw_info_response(path)
            group_ids = {row.group_id for row in rows}
            if len(group_ids) != 1:
                raise ValueError(f"{path}: expected one DrawInfoList GroupId per response, got {sorted(group_ids)}.")
            group_id = next(iter(group_ids))
            first_info_path_by_group.setdefault(group_id, path)

        return group_path, [first_info_path_by_group[group_id] for group_id in sorted(first_info_path_by_group)], dump_dir.name

    if args.draw_group_list is None:
        raise ValueError("Either --draw-dump-dir or --draw-group-list is required.")
    if not args.draw_info_list:
        raise ValueError("At least one --draw-info-list is required when --draw-dump-dir is not used.")
    return args.draw_group_list, args.draw_info_list, args.draw_group_list.name


def load_draw_tables(group_path: Path, info_paths: list[Path]) -> tuple[list[DrawGroupRow], list[DrawInfoRow], list[DrawAdjustActivityRow]]:
    groups, adjust_rows = load_draw_group_response(group_path)

    infos: list[DrawInfoRow] = []
    seen_draw_ids: dict[int, Path] = {}
    for path in info_paths:
        for row in load_draw_info_response(path):
            if row.id in seen_draw_ids:
                raise ValueError(f"{path}: duplicate DrawInfoList Id {row.id}; first seen in {seen_draw_ids[row.id]}.")
            seen_draw_ids[row.id] = path
            infos.append(row)

    groups_by_id = {group.id: group for group in groups}
    info_group_ids = {info.group_id for info in infos}
    missing_info_groups = sorted(set(groups_by_id) - info_group_ids)
    extra_info_groups = sorted(info_group_ids - set(groups_by_id))
    if missing_info_groups:
        raise ValueError(f"{group_path}: missing DrawInfoList responses for group ids {missing_info_groups}.")
    if extra_info_groups:
        raise ValueError(f"{group_path}: DrawInfoList responses contain unknown group ids {extra_info_groups}.")

    draw_info_ids = {info.id for info in infos}
    for group in groups:
        referenced_ids = [
            draw_id
            for draw_id in [*group.optional_draw_id_list, *group.tag_black_list_draw_ids, *group.default_use_draw_id_dict.values()]
            if draw_id > 0
        ]
        missing_draw_ids = sorted({draw_id for draw_id in referenced_ids if draw_id not in draw_info_ids})
        if missing_draw_ids:
            raise ValueError(f"{group_path}: draw group {group.id} references missing DrawInfo ids {missing_draw_ids}.")

    infos.sort(key=lambda info: (groups_by_id[info.group_id].order, info.group_id, info.id))
    return groups, infos, adjust_rows


def csharp_bool(value: bool) -> str:
    return "true" if value else "false"


def csharp_string(value: str) -> str:
    return json.dumps(value, ensure_ascii=False)


def csharp_int_list(values: list[int]) -> str:
    return "[]" if not values else "[" + ", ".join(str(value) for value in values) + "]"


def csharp_int_dict(values: dict[int, int]) -> str:
    return csharp_dict(values, value_formatter=str)


def csharp_string_dict(values: dict[int, str]) -> str:
    return csharp_dict(values, value_formatter=csharp_string)


def csharp_dict(values: dict[int, Any], *, value_formatter: Any) -> str:
    if not values:
        return "new()"
    entries = ", ".join(f"[{key}] = {value_formatter(value)}" for key, value in sorted(values.items()))
    return f"new() {{ {entries} }}"


def generate_source(source_label: str, groups: list[DrawGroupRow], infos: list[DrawInfoRow], adjust_rows: list[DrawAdjustActivityRow]) -> str:
    lines: list[str] = [
        "// <auto-generated>",
        f"// Generated by Scripts/generate_current_draw_tables.py from {source_label}.",
        "// Do not edit by hand; regenerate from the latest retail draw response dumps.",
        "// </auto-generated>",
        "using AscNet.GameServer.Handlers;",
        "",
        "namespace AscNet.GameServer.Game",
        "{",
        "    internal partial class DrawManager",
        "    {",
        "        private static DrawGroupDefinition[] BuildGeneratedDrawGroupDefinitions()",
        "        {",
        "            return",
        "            [",
    ]

    for index, group in enumerate(groups):
        comma = "," if index < len(groups) - 1 else ""
        lines.append(
            "                "
            + "new("
            + ", ".join(
                [
                    str(group.id),
                    str(group.tag),
                    str(group.order),
                    str(group.priority),
                    str(group.use_item_id),
                    str(group.max_bottom_times),
                    str(group.type),
                    csharp_string(group.banner),
                    csharp_int_dict(group.default_use_draw_id_dict),
                    csharp_int_list(group.optional_draw_id_list),
                    csharp_int_list(group.tag_black_list_draw_ids),
                    str(group.banner_begin_time),
                    str(group.banner_end_time),
                    str(group.condition_id),
                    str(group.show_predict_type),
                    str(group.start_time),
                    str(group.end_time),
                ]
            )
            + f"){comma}"
        )

    lines.extend(
        [
            "            ];",
            "        }",
            "",
            "        private static DrawInfoTemplate[] BuildGeneratedRetailDrawInfoTemplates()",
            "        {",
            "            return",
            "            [",
        ]
    )

    for index, info in enumerate(infos):
        comma = "," if index < len(infos) - 1 else ""
        lines.append(
            "                "
            + "new("
            + ", ".join(
                [
                    str(info.id),
                    str(info.group_id),
                    str(info.draw_type),
                    str(info.use_item_id),
                    str(info.use_item_count),
                    str(info.base_today_count),
                    str(info.base_total_count),
                    str(info.bottom_times),
                    str(info.max_bottom_times),
                    str(info.start_time),
                    str(info.end_time),
                    csharp_string(info.banner),
                    csharp_string_dict(info.resources),
                    csharp_int_dict(info.resource_ids),
                    csharp_int_list(info.btn_draw_count),
                    csharp_int_list(info.purchase_ui_type),
                    csharp_int_list(info.purchase_id),
                    csharp_int_list(info.ex_purchase_ids),
                    str(info.capacity_check_type),
                    str(info.group_sub_type),
                    csharp_bool(info.is_trigger_specified),
                    csharp_bool(info.is_show_shop),
                    csharp_bool(info.is_show_bubble),
                    str(info.use_ten_draw_on_sale_times),
                    str(info.daily_limit_times),
                    str(info.activity_limit_times),
                    str(info.show_priority),
                    str(info.up_goods_id),
                ]
            )
            + f"){comma}"
        )

    lines.extend(
        [
            "            ];",
            "        }",
            "",
            "        private static List<DrawAdjustActivityInfo> BuildGeneratedDrawAdjustActivityInfos()",
            "        {",
        ]
    )

    if adjust_rows:
        lines.extend(["            return", "            ["])
        for index, row in enumerate(adjust_rows):
            comma = "," if index < len(adjust_rows) - 1 else ""
            lines.extend(
                [
                    "                new()",
                    "                {",
                    f"                    TargetTimes = {row.target_times},",
                    f"                    TargetId = {row.target_id},",
                    f"                    ActivityStatus = {row.activity_status},",
                    f"                    ActivityId = {row.activity_id},",
                    f"                    StartTime = {row.start_time},",
                    f"                    EndTime = {row.end_time},",
                    f"                    AdjustTimes = {row.adjust_times},",
                    f"                    DrawGroupId = {row.draw_group_id},",
                    f"                    TargetTemplateIds = {csharp_int_list(row.target_template_ids)},",
                    f"                    SourceTemplateIds = {csharp_int_list(row.source_template_ids)},",
                    f"                    EffectTargetTemplateIds = {csharp_int_list(row.effect_target_template_ids)}",
                    f"                }}{comma}",
                ]
            )
        lines.append("            ];")
    else:
        lines.append("            return [];")

    lines.extend(
        [
            "        }",
            "    }",
            "}",
        ]
    )
    return "\n".join(lines) + "\n"


def parse_args(argv: list[str]) -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description=(
            "Generate the current server draw group/info tables from decoded retail draw response JSON dumps. "
            "This updates the single latest config, not versioned season bundles."
        )
    )
    parser.add_argument(
        "--draw-dump-dir",
        type=Path,
        help="Directory containing decoded *-DrawGetDrawGroupListResponse.json and *-DrawGetDrawInfoListResponse.json files; selects the latest group response and earliest info response per group.",
    )
    parser.add_argument(
        "--draw-group-list",
        type=Path,
        help="Path to decoded retail DrawGetDrawGroupListResponse.json when not using --draw-dump-dir.",
    )
    parser.add_argument(
        "--draw-info-list",
        action="append",
        type=Path,
        default=[],
        help="Path to decoded retail DrawGetDrawInfoListResponse.json; repeat once per draw group when not using --draw-dump-dir.",
    )
    parser.add_argument(
        "--output",
        type=Path,
        default=DEFAULT_OUTPUT,
        help=f"Generated C# output path. Default: {DEFAULT_OUTPUT.relative_to(REPO_ROOT).as_posix()}.",
    )
    parser.add_argument(
        "--check",
        action="store_true",
        help="Do not write; exit non-zero if the output file is missing or stale.",
    )
    return parser.parse_args(argv)


def main(argv: list[str]) -> int:
    args = parse_args(argv)
    try:
        group_path, info_paths, source_label = resolve_input_paths(args)
        groups, infos, adjust_rows = load_draw_tables(group_path, info_paths)
    except ValueError as exception:
        print(exception, file=sys.stderr)
        return 2

    output = args.output
    generated = generate_source(source_label, groups, infos, adjust_rows)

    if args.check:
        if not output.exists():
            print(f"stale: {output} does not exist", file=sys.stderr)
            return 1
        existing = output.read_text(encoding="utf-8")
        if existing != generated:
            print(f"stale: {output} differs from {source_label}", file=sys.stderr)
            print(f"draw_groups={len(groups)} draw_infos={len(infos)} draw_adjust_activities={len(adjust_rows)}", file=sys.stderr)
            return 1
        print(f"up-to-date: {output}")
        print(f"draw_groups={len(groups)} draw_infos={len(infos)} draw_adjust_activities={len(adjust_rows)}")
        return 0

    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text(generated, encoding="utf-8")
    print(f"generated: {output}")
    print(f"draw_groups={len(groups)} draw_infos={len(infos)} draw_adjust_activities={len(adjust_rows)}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main(sys.argv[1:]))
