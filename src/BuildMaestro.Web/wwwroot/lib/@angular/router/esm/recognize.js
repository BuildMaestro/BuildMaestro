import { Observable } from 'rxjs/Observable';
import { of } from 'rxjs/observable/of';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from './router_state';
import { PRIMARY_OUTLET } from './shared';
import { mapChildrenIntoArray } from './url_tree';
import { last, merge } from './utils/collection';
import { TreeNode } from './utils/tree';
class NoMatch {
    constructor(segment = null) {
        this.segment = segment;
    }
}
export function recognize(rootComponentType, config, urlTree, url) {
    try {
        const children = processSegment(config, urlTree.root, PRIMARY_OUTLET);
        const root = new ActivatedRouteSnapshot([], {}, PRIMARY_OUTLET, rootComponentType, null, urlTree.root, -1);
        const rootNode = new TreeNode(root, children);
        return of(new RouterStateSnapshot(url, rootNode, urlTree.queryParams, urlTree.fragment));
    }
    catch (e) {
        if (e instanceof NoMatch) {
            return new Observable((obs) => obs.error(new Error(`Cannot match any routes: '${e.segment}'`)));
        }
        else {
            return new Observable((obs) => obs.error(e));
        }
    }
}
function processSegment(config, segment, outlet) {
    if (segment.pathsWithParams.length === 0 && Object.keys(segment.children).length > 0) {
        return processSegmentChildren(config, segment);
    }
    else {
        return [processPathsWithParams(config, segment, 0, segment.pathsWithParams, outlet)];
    }
}
function processSegmentChildren(config, segment) {
    const children = mapChildrenIntoArray(segment, (child, childOutlet) => processSegment(config, child, childOutlet));
    checkOutletNameUniqueness(children);
    sortActivatedRouteSnapshots(children);
    return children;
}
function sortActivatedRouteSnapshots(nodes) {
    nodes.sort((a, b) => {
        if (a.value.outlet === PRIMARY_OUTLET)
            return -1;
        if (b.value.outlet === PRIMARY_OUTLET)
            return 1;
        return a.value.outlet.localeCompare(b.value.outlet);
    });
}
function processPathsWithParams(config, segment, pathIndex, paths, outlet) {
    for (let r of config) {
        try {
            return processPathsWithParamsAgainstRoute(r, segment, pathIndex, paths, outlet);
        }
        catch (e) {
            if (!(e instanceof NoMatch))
                throw e;
        }
    }
    throw new NoMatch(segment);
}
function processPathsWithParamsAgainstRoute(route, segment, pathIndex, paths, outlet) {
    if (route.redirectTo)
        throw new NoMatch();
    if ((route.outlet ? route.outlet : PRIMARY_OUTLET) !== outlet)
        throw new NoMatch();
    if (route.path === '**') {
        const params = paths.length > 0 ? last(paths).parameters : {};
        const snapshot = new ActivatedRouteSnapshot(paths, params, outlet, route.component, route, segment, -1);
        return new TreeNode(snapshot, []);
    }
    const { consumedPaths, parameters, lastChild } = match(segment, route, paths);
    const snapshot = new ActivatedRouteSnapshot(consumedPaths, parameters, outlet, route.component, route, segment, pathIndex + lastChild - 1);
    const slicedPath = paths.slice(lastChild);
    const childConfig = route.children ? route.children : [];
    if (childConfig.length === 0 && slicedPath.length === 0) {
        return new TreeNode(snapshot, []);
    }
    else if (slicedPath.length === 0 && Object.keys(segment.children).length > 0) {
        const children = processSegmentChildren(childConfig, segment);
        return new TreeNode(snapshot, children);
    }
    else {
        const child = processPathsWithParams(childConfig, segment, pathIndex + lastChild, slicedPath, PRIMARY_OUTLET);
        return new TreeNode(snapshot, [child]);
    }
}
function match(segment, route, paths) {
    if (route.index || route.path === '' || route.path === '/') {
        if (route.terminal && (Object.keys(segment.children).length > 0 || paths.length > 0)) {
            throw new NoMatch();
        }
        else {
            return { consumedPaths: [], lastChild: 0, parameters: {} };
        }
    }
    const path = route.path.startsWith('/') ? route.path.substring(1) : route.path;
    const parts = path.split('/');
    const posParameters = {};
    const consumedPaths = [];
    let currentIndex = 0;
    for (let i = 0; i < parts.length; ++i) {
        if (currentIndex >= paths.length)
            throw new NoMatch();
        const current = paths[currentIndex];
        const p = parts[i];
        const isPosParam = p.startsWith(':');
        if (!isPosParam && p !== current.path)
            throw new NoMatch();
        if (isPosParam) {
            posParameters[p.substring(1)] = current.path;
        }
        consumedPaths.push(current);
        currentIndex++;
    }
    if (route.terminal && (Object.keys(segment.children).length > 0 || currentIndex < paths.length)) {
        throw new NoMatch();
    }
    const parameters = merge(posParameters, consumedPaths[consumedPaths.length - 1].parameters);
    return { consumedPaths, lastChild: currentIndex, parameters };
}
function checkOutletNameUniqueness(nodes) {
    const names = {};
    nodes.forEach(n => {
        let routeWithSameOutletName = names[n.value.outlet];
        if (routeWithSameOutletName) {
            const p = routeWithSameOutletName.url.map(s => s.toString()).join('/');
            const c = n.value.url.map(s => s.toString()).join('/');
            throw new Error(`Two segments cannot have the same outlet name: '${p}' and '${c}'.`);
        }
        names[n.value.outlet] = n.value;
    });
}
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoicmVjb2duaXplLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vLi4vc3JjL3JlY29nbml6ZS50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiT0FDTyxFQUFDLFVBQVUsRUFBQyxNQUFNLGlCQUFpQjtPQUVuQyxFQUFDLEVBQUUsRUFBRSxNQUFNLG9CQUFvQjtPQUcvQixFQUFDLHNCQUFzQixFQUFFLG1CQUFtQixFQUFDLE1BQU0sZ0JBQWdCO09BQ25FLEVBQUMsY0FBYyxFQUFDLE1BQU0sVUFBVTtPQUNoQyxFQUF5QyxvQkFBb0IsRUFBQyxNQUFNLFlBQVk7T0FDaEYsRUFBQyxJQUFJLEVBQUUsS0FBSyxFQUFDLE1BQU0sb0JBQW9CO09BQ3ZDLEVBQUMsUUFBUSxFQUFDLE1BQU0sY0FBYztBQUVyQztJQUNFLFlBQW1CLE9BQU8sR0FBZSxJQUFJO1FBQTFCLFlBQU8sR0FBUCxPQUFPLENBQW1CO0lBQUcsQ0FBQztBQUNuRCxDQUFDO0FBRUQsMEJBQ0ksaUJBQXVCLEVBQUUsTUFBb0IsRUFBRSxPQUFnQixFQUMvRCxHQUFXO0lBQ2IsSUFBSSxDQUFDO1FBQ0gsTUFBTSxRQUFRLEdBQUcsY0FBYyxDQUFDLE1BQU0sRUFBRSxPQUFPLENBQUMsSUFBSSxFQUFFLGNBQWMsQ0FBQyxDQUFDO1FBQ3RFLE1BQU0sSUFBSSxHQUFHLElBQUksc0JBQXNCLENBQ25DLEVBQUUsRUFBRSxFQUFFLEVBQUUsY0FBYyxFQUFFLGlCQUFpQixFQUFFLElBQUksRUFBRSxPQUFPLENBQUMsSUFBSSxFQUFFLENBQUMsQ0FBQyxDQUFDLENBQUM7UUFDdkUsTUFBTSxRQUFRLEdBQUcsSUFBSSxRQUFRLENBQXlCLElBQUksRUFBRSxRQUFRLENBQUMsQ0FBQztRQUN0RSxNQUFNLENBQUMsRUFBRSxDQUFFLElBQUksbUJBQW1CLENBQUMsR0FBRyxFQUFFLFFBQVEsRUFBRSxPQUFPLENBQUMsV0FBVyxFQUFFLE9BQU8sQ0FBQyxRQUFRLENBQUMsQ0FBQyxDQUFDO0lBQzVGLENBQUU7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QUMsSUFBSSxDQUFDLEdBQUcsQ0FBQyxDQUFDO1lBQ3ZFLE1BQU0sQ0FBQyxHQUFHLENBQUMsQ0FBQyxLQUFLLENBQUMsR0FBRyxDQUFDLEdBQUcsQ0FBQyxDQUFDLElBQUksQ0FBQyxDQUFDLFFBQVEsRUFBRSxDQUFDLENBQUMsSUFBSSxDQUFDLEdBQUcsQ0FBQyxDQUFDO1lBQ3ZELE1BQU0sSUFBSSxLQUFLLENBQUMsbURBQW1ELENBQUMsVUFBVSxDQUFDLElBQUksQ0FBQyxDQUFDO1FBQ3ZGLENBQUM7UUFDRCxLQUFLLENBQUMsQ0FBQyxDQUFDLEtBQUssQ0FBQyxNQUFNLENBQUMsR0FBRyxDQUFDLENBQUMsS0FBSyxDQUFDO0lBQ2xDLENBQUMsQ0FBQyxDQUFDO0FBQ0wsQ0FBQyIsInNvdXJjZXNDb250ZW50IjpbImltcG9ydCB7VHlwZX0gZnJvbSAnQGFuZ3VsYXIvY29yZSc7XG5pbXBvcnQge09ic2VydmFibGV9IGZyb20gJ3J4anMvT2JzZXJ2YWJsZSc7XG5pbXBvcnQge09ic2VydmVyfSBmcm9tICdyeGpzL09ic2VydmVyJztcbmltcG9ydCB7b2YgfSBmcm9tICdyeGpzL29ic2VydmFibGUvb2YnO1xuXG5pbXBvcnQge1JvdXRlLCBSb3V0ZXJDb25maWd9IGZyb20gJy4vY29uZmlnJztcbmltcG9ydCB7QWN0aXZhdGVkUm91dGVTbmFwc2hvdCwgUm91dGVyU3RhdGVTbmFwc2hvdH0gZnJvbSAnLi9yb3V0ZXJfc3RhdGUnO1xuaW1wb3J0IHtQUklNQVJZX09VVExFVH0gZnJvbSAnLi9zaGFyZWQnO1xuaW1wb3J0IHtVcmxQYXRoV2l0aFBhcmFtcywgVXJsU2VnbWVudCwgVXJsVHJlZSwgbWFwQ2hpbGRyZW5JbnRvQXJyYXl9IGZyb20gJy4vdXJsX3RyZWUnO1xuaW1wb3J0IHtsYXN0LCBtZXJnZX0gZnJvbSAnLi91dGlscy9jb2xsZWN0aW9uJztcbmltcG9ydCB7VHJlZU5vZGV9IGZyb20gJy4vdXRpbHMvdHJlZSc7XG5cbmNsYXNzIE5vTWF0Y2gge1xuICBjb25zdHJ1Y3RvcihwdWJsaWMgc2VnbWVudDogVXJsU2VnbWVudCA9IG51bGwpIHt9XG59XG5cbmV4cG9ydCBmdW5jdGlvbiByZWNvZ25pemUoXG4gICAgcm9vdENvbXBvbmVudFR5cGU6IFR5cGUsIGNvbmZpZzogUm91dGVyQ29uZmlnLCB1cmxUcmVlOiBVcmxUcmVlLFxuICAgIHVybDogc3RyaW5nKTogT2JzZXJ2YWJsZTxSb3V0ZXJTdGF0ZVNuYXBzaG90PiB7XG4gIHRyeSB7XG4gICAgY29uc3QgY2hpbGRyZW4gPSBwcm9jZXNzU2VnbWVudChjb25maWcsIHVybFRyZWUucm9vdCwgUFJJTUFSWV9PVVRMRVQpO1xuICAgIGNvbnN0IHJvb3QgPSBuZXcgQWN0aXZhdGVkUm91dGVTbmFwc2hvdChcbiAgICAgICAgW10sIHt9LCBQUklNQVJZX09VVExFVCwgcm9vdENvbXBvbmVudFR5cGUsIG51bGwsIHVybFRyZWUucm9vdCwgLTEpO1xuICAgIGNvbnN0IHJvb3ROb2RlID0gbmV3IFRyZWVOb2RlPEFjdGl2YXRlZFJvdXRlU25hcHNob3Q+KHJvb3QsIGNoaWxkcmVuKTtcbiAgICByZXR1cm4gb2YgKG5ldyBSb3V0ZXJTdGF0ZVNuYXBzaG90KHVybCwgcm9vdE5vZGUsIHVybFRyZWUucXVlcnlQYXJhbXMsIHVybFRyZWUuZnJhZ21lbnQpKTtcbiAgfSBjYXRjaCAoZSkge1xuICAgIGlmIChlIGluc3RhbmNlb2YgTm9NYXRjaCkge1xuICAgICAgcmV0dXJuIG5ldyBPYnNlcnZhYmxlPFJvdXRlclN0YXRlU25hcHNob3Q+KFxuICAgICAgICAgIChvYnM6IE9ic2VydmVyPFJvdXRlclN0YXRlU25hcHNob3Q+KSA9PlxuICAgICAgICAgICAgICBvYnMuZXJyb3IobmV3IEVycm9yKGBDYW5ub3QgbWF0Y2ggYW55IHJvdXRlczogJyR7ZS5zZWdtZW50fSdgKSkpO1xuICAgIH0gZWxzZSB7XG4gICAgICByZXR1cm4gbmV3IE9ic2VydmFibGU8Um91dGVyU3RhdGVTbmFwc2hvdD4oXG4gICAgICAgICAgKG9iczogT2JzZXJ2ZXI8Um91dGVyU3RhdGVTbmFwc2hvdD4pID0+IG9icy5lcnJvcihlKSk7XG4gICAgfVxuICB9XG59XG5cbmZ1bmN0aW9uIHByb2Nlc3NTZWdtZW50KFxuICAgIGNvbmZpZzogUm91dGVbXSwgc2VnbWVudDogVXJsU2VnbWVudCwgb3V0bGV0OiBzdHJpbmcpOiBUcmVlTm9kZTxBY3RpdmF0ZWRSb3V0ZVNuYXBzaG90PltdIHtcbiAgaWYgKHNlZ21lbnQucGF0aHNXaXRoUGFyYW1zLmxlbmd0aCA9PT0gMCAmJiBPYmplY3Qua2V5cyhzZWdtZW50LmNoaWxkcmVuKS5sZW5ndGggPiAwKSB7XG4gICAgcmV0dXJuIHByb2Nlc3NTZWdtZW50Q2hpbGRyZW4oY29uZmlnLCBzZWdtZW50KTtcbiAgfSBlbHNlIHtcbiAgICByZXR1cm4gW3Byb2Nlc3NQYXRoc1dpdGhQYXJhbXMoY29uZmlnLCBzZWdtZW50LCAwLCBzZWdtZW50LnBhdGhzV2l0aFBhcmFtcywgb3V0bGV0KV07XG4gIH1cbn1cblxuZnVuY3Rpb24gcHJvY2Vzc1NlZ21lbnRDaGlsZHJlbihcbiAgICBjb25maWc6IFJvdXRlW10sIHNlZ21lbnQ6IFVybFNlZ21lbnQpOiBUcmVlTm9kZTxBY3RpdmF0ZWRSb3V0ZVNuYXBzaG90PltdIHtcbiAgY29uc3QgY2hpbGRyZW4gPSBtYXBDaGlsZHJlbkludG9BcnJheShcbiAgICAgIHNlZ21lbnQsIChjaGlsZCwgY2hpbGRPdXRsZXQpID0+IHByb2Nlc3NTZWdtZW50KGNvbmZpZywgY2hpbGQsIGNoaWxkT3V0bGV0KSk7XG4gIGNoZWNrT3V0bGV0TmFtZVVuaXF1ZW5lc3MoY2hpbGRyZW4pO1xuICBzb3J0QWN0aXZhdGVkUm91dGVTbmFwc2hvdHMoY2hpbGRyZW4pO1xuICByZXR1cm4gY2hpbGRyZW47XG59XG5cbmZ1bmN0aW9uIHNvcnRBY3RpdmF0ZWRSb3V0ZVNuYXBzaG90cyhub2RlczogVHJlZU5vZGU8QWN0aXZhdGVkUm91dGVTbmFwc2hvdD5bXSk6IHZvaWQge1xuICBub2Rlcy5zb3J0KChhLCBiKSA9PiB7XG4gICAgaWYgKGEudmFsdWUub3V0bGV0ID09PSBQUklNQVJZX09VVExFVCkgcmV0dXJuIC0xO1xuICAgIGlmIChiLnZhbHVlLm91dGxldCA9PT0gUFJJTUFSWV9PVVRMRVQpIHJldHVybiAxO1xuICAgIHJldHVybiBhLnZhbHVlLm91dGxldC5sb2NhbGVDb21wYXJlKGIudmFsdWUub3V0bGV0KTtcbiAgfSk7XG59XG5cbmZ1bmN0aW9uIHByb2Nlc3NQYXRoc1dpdGhQYXJhbXMoXG4gICAgY29uZmlnOiBSb3V0ZVtdLCBzZWdtZW50OiBVcmxTZWdtZW50LCBwYXRoSW5kZXg6IG51bWJlciwgcGF0aHM6IFVybFBhdGhXaXRoUGFyYW1zW10sXG4gICAgb3V0bGV0OiBzdHJpbmcpOiBUcmVlTm9kZTxBY3RpdmF0ZWRSb3V0ZVNuYXBzaG90PiB7XG4gIGZvciAobGV0IHIgb2YgY29uZmlnKSB7XG4gICAgdHJ5IHtcbiAgICAgIHJldHVybiBwcm9jZXNzUGF0aHNXaXRoUGFyYW1zQWdhaW5zdFJvdXRlKHIsIHNlZ21lbnQsIHBhdGhJbmRleCwgcGF0aHMsIG91dGxldCk7XG4gICAgfSBjYXRjaCAoZSkge1xuICAgICAgaWYgKCEoZSBpbnN0YW5jZW9mIE5vTWF0Y2gpKSB0aHJvdyBlO1xuICAgIH1cbiAgfVxuICB0aHJvdyBuZXcgTm9NYXRjaChzZWdtZW50KTtcbn1cblxuZnVuY3Rpb24gcHJvY2Vzc1BhdGhzV2l0aFBhcmFtc0FnYWluc3RSb3V0ZShcbiAgICByb3V0ZTogUm91dGUsIHNlZ21lbnQ6IFVybFNlZ21lbnQsIHBhdGhJbmRleDogbnVtYmVyLCBwYXRoczogVXJsUGF0aFdpdGhQYXJhbXNbXSxcbiAgICBvdXRsZXQ6IHN0cmluZyk6IFRyZWVOb2RlPEFjdGl2YXRlZFJvdXRlU25hcHNob3Q+IHtcbiAgaWYgKHJvdXRlLnJlZGlyZWN0VG8pIHRocm93IG5ldyBOb01hdGNoKCk7XG4gIGlmICgocm91dGUub3V0bGV0ID8gcm91dGUub3V0bGV0IDogUFJJTUFSWV9PVVRMRVQpICE9PSBvdXRsZXQpIHRocm93IG5ldyBOb01hdGNoKCk7XG5cbiAgaWYgKHJvdXRlLnBhdGggPT09ICcqKicpIHtcbiAgICBjb25zdCBwYXJhbXMgPSBwYXRocy5sZW5ndGggPiAwID8gbGFzdChwYXRocykucGFyYW1ldGVycyA6IHt9O1xuICAgIGNvbnN0IHNuYXBzaG90ID1cbiAgICAgICAgbmV3IEFjdGl2YXRlZFJvdXRlU25hcHNob3QocGF0aHMsIHBhcmFtcywgb3V0bGV0LCByb3V0ZS5jb21wb25lbnQsIHJvdXRlLCBzZWdtZW50LCAtMSk7XG4gICAgcmV0dXJuIG5ldyBUcmVlTm9kZTxBY3RpdmF0ZWRSb3V0ZVNuYXBzaG90PihzbmFwc2hvdCwgW10pO1xuICB9XG5cbiAgY29uc3Qge2NvbnN1bWVkUGF0aHMsIHBhcmFtZXRlcnMsIGxhc3RDaGlsZH0gPSBtYXRjaChzZWdtZW50LCByb3V0ZSwgcGF0aHMpO1xuXG4gIGNvbnN0IHNuYXBzaG90ID0gbmV3IEFjdGl2YXRlZFJvdXRlU25hcHNob3QoXG4gICAgICBjb25zdW1lZFBhdGhzLCBwYXJhbWV0ZXJzLCBvdXRsZXQsIHJvdXRlLmNvbXBvbmVudCwgcm91dGUsIHNlZ21lbnQsXG4gICAgICBwYXRoSW5kZXggKyBsYXN0Q2hpbGQgLSAxKTtcbiAgY29uc3Qgc2xpY2VkUGF0aCA9IHBhdGhzLnNsaWNlKGxhc3RDaGlsZCk7XG4gIGNvbnN0IGNoaWxkQ29uZmlnID0gcm91dGUuY2hpbGRyZW4gPyByb3V0ZS5jaGlsZHJlbiA6IFtdO1xuXG4gIGlmIChjaGlsZENvbmZpZy5sZW5ndGggPT09IDAgJiYgc2xpY2VkUGF0aC5sZW5ndGggPT09IDApIHtcbiAgICByZXR1cm4gbmV3IFRyZWVOb2RlPEFjdGl2YXRlZFJvdXRlU25hcHNob3Q+KHNuYXBzaG90LCBbXSk7XG5cbiAgICAvLyBUT0RPOiBjaGVjayB0aGF0IHRoZSByaWdodCBzZWdtZW50IGlzIHByZXNlbnRcbiAgfSBlbHNlIGlmIChzbGljZWRQYXRoLmxlbmd0aCA9PT0gMCAmJiBPYmplY3Qua2V5cyhzZWdtZW50LmNoaWxkcmVuKS5sZW5ndGggPiAwKSB7XG4gICAgY29uc3QgY2hpbGRyZW4gPSBwcm9jZXNzU2VnbWVudENoaWxkcmVuKGNoaWxkQ29uZmlnLCBzZWdtZW50KTtcbiAgICByZXR1cm4gbmV3IFRyZWVOb2RlPEFjdGl2YXRlZFJvdXRlU25hcHNob3Q+KHNuYXBzaG90LCBjaGlsZHJlbik7XG5cbiAgfSBlbHNlIHtcbiAgICBjb25zdCBjaGlsZCA9IHByb2Nlc3NQYXRoc1dpdGhQYXJhbXMoXG4gICAgICAgIGNoaWxkQ29uZmlnLCBzZWdtZW50LCBwYXRoSW5kZXggKyBsYXN0Q2hpbGQsIHNsaWNlZFBhdGgsIFBSSU1BUllfT1VUTEVUKTtcbiAgICByZXR1cm4gbmV3IFRyZWVOb2RlPEFjdGl2YXRlZFJvdXRlU25hcHNob3Q+KHNuYXBzaG90LCBbY2hpbGRdKTtcbiAgfVxufVxuXG5mdW5jdGlvbiBtYXRjaChzZWdtZW50OiBVcmxTZWdtZW50LCByb3V0ZTogUm91dGUsIHBhdGhzOiBVcmxQYXRoV2l0aFBhcmFtc1tdKSB7XG4gIGlmIChyb3V0ZS5pbmRleCB8fCByb3V0ZS5wYXRoID09PSAnJyB8fCByb3V0ZS5wYXRoID09PSAnLycpIHtcbiAgICBpZiAocm91dGUudGVybWluYWwgJiYgKE9iamVjdC5rZXlzKHNlZ21lbnQuY2hpbGRyZW4pLmxlbmd0aCA+IDAgfHwgcGF0aHMubGVuZ3RoID4gMCkpIHtcbiAgICAgIHRocm93IG5ldyBOb01hdGNoKCk7XG4gICAgfSBlbHNlIHtcbiAgICAgIHJldHVybiB7Y29uc3VtZWRQYXRoczogW10sIGxhc3RDaGlsZDogMCwgcGFyYW1ldGVyczoge319O1xuICAgIH1cbiAgfVxuXG4gIGNvbnN0IHBhdGggPSByb3V0ZS5wYXRoLnN0YXJ0c1dpdGgoJy8nKSA/IHJvdXRlLnBhdGguc3Vic3RyaW5nKDEpIDogcm91dGUucGF0aDtcbiAgY29uc3QgcGFydHMgPSBwYXRoLnNwbGl0KCcvJyk7XG4gIGNvbnN0IHBvc1BhcmFtZXRlcnM6IHtba2V5OiBzdHJpbmddOiBhbnl9ID0ge307XG4gIGNvbnN0IGNvbnN1bWVkUGF0aHM6IFVybFBhdGhXaXRoUGFyYW1zW10gPSBbXTtcblxuICBsZXQgY3VycmVudEluZGV4ID0gMDtcblxuICBmb3IgKGxldCBpID0gMDsgaSA8IHBhcnRzLmxlbmd0aDsgKytpKSB7XG4gICAgaWYgKGN1cnJlbnRJbmRleCA+PSBwYXRocy5sZW5ndGgpIHRocm93IG5ldyBOb01hdGNoKCk7XG4gICAgY29uc3QgY3VycmVudCA9IHBhdGhzW2N1cnJlbnRJbmRleF07XG5cbiAgICBjb25zdCBwID0gcGFydHNbaV07XG4gICAgY29uc3QgaXNQb3NQYXJhbSA9IHAuc3RhcnRzV2l0aCgnOicpO1xuXG4gICAgaWYgKCFpc1Bvc1BhcmFtICYmIHAgIT09IGN1cnJlbnQucGF0aCkgdGhyb3cgbmV3IE5vTWF0Y2goKTtcbiAgICBpZiAoaXNQb3NQYXJhbSkge1xuICAgICAgcG9zUGFyYW1ldGVyc1twLnN1YnN0cmluZygxKV0gPSBjdXJyZW50LnBhdGg7XG4gICAgfVxuICAgIGNvbnN1bWVkUGF0aHMucHVzaChjdXJyZW50KTtcbiAgICBjdXJyZW50SW5kZXgrKztcbiAgfVxuXG4gIGlmIChyb3V0ZS50ZXJtaW5hbCAmJiAoT2JqZWN0LmtleXMoc2VnbWVudC5jaGlsZHJlbikubGVuZ3RoID4gMCB8fCBjdXJyZW50SW5kZXggPCBwYXRocy5sZW5ndGgpKSB7XG4gICAgdGhyb3cgbmV3IE5vTWF0Y2goKTtcbiAgfVxuXG4gIGNvbnN0IHBhcmFtZXRlcnMgPSBtZXJnZShwb3NQYXJhbWV0ZXJzLCBjb25zdW1lZFBhdGhzW2NvbnN1bWVkUGF0aHMubGVuZ3RoIC0gMV0ucGFyYW1ldGVycyk7XG4gIHJldHVybiB7Y29uc3VtZWRQYXRocywgbGFzdENoaWxkOiBjdXJyZW50SW5kZXgsIHBhcmFtZXRlcnN9O1xufVxuXG5mdW5jdGlvbiBjaGVja091dGxldE5hbWVVbmlxdWVuZXNzKG5vZGVzOiBUcmVlTm9kZTxBY3RpdmF0ZWRSb3V0ZVNuYXBzaG90PltdKTogdm9pZCB7XG4gIGNvbnN0IG5hbWVzOiB7W2s6IHN0cmluZ106IEFjdGl2YXRlZFJvdXRlU25hcHNob3R9ID0ge307XG4gIG5vZGVzLmZvckVhY2gobiA9PiB7XG4gICAgbGV0IHJvdXRlV2l0aFNhbWVPdXRsZXROYW1lID0gbmFtZXNbbi52YWx1ZS5vdXRsZXRdO1xuICAgIGlmIChyb3V0ZVdpdGhTYW1lT3V0bGV0TmFtZSkge1xuICAgICAgY29uc3QgcCA9IHJvdXRlV2l0aFNhbWVPdXRsZXROYW1lLnVybC5tYXAocyA9PiBzLnRvU3RyaW5nKCkpLmpvaW4oJy8nKTtcbiAgICAgIGNvbnN0IGMgPSBuLnZhbHVlLnVybC5tYXAocyA9PiBzLnRvU3RyaW5nKCkpLmpvaW4oJy8nKTtcbiAgICAgIHRocm93IG5ldyBFcnJvcihgVHdvIHNlZ21lbnRzIGNhbm5vdCBoYXZlIHRoZSBzYW1lIG91dGxldCBuYW1lOiAnJHtwfScgYW5kICcke2N9Jy5gKTtcbiAgICB9XG4gICAgbmFtZXNbbi52YWx1ZS5vdXRsZXRdID0gbi52YWx1ZTtcbiAgfSk7XG59Il19