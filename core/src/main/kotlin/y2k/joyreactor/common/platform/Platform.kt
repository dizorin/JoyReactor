package y2k.joyreactor.common.platform

import com.j256.ormlite.support.ConnectionSource
import rx.Completable
import rx.Single
import y2k.joyreactor.common.async.CompletableContinuation
import y2k.joyreactor.services.ReportService
import java.io.File

/**
 * Created by y2k on 29/09/15.
 */
interface Platform {

    val currentDirectory: File

    val navigator: NavigationService

    fun loadFromBundle(name: String, ext: String): ByteArray

    @Deprecated("")
    fun saveToGallery_(imageFile: File): Completable

    fun saveToGallery(imageFile: File): CompletableContinuation<*>

    fun buildConnection(file: File): ConnectionSource

    fun <T> decodeImage(path: File): T

    fun makeReportService(): ReportService

    @Deprecated("")
    fun createTmpThumbnail_(videoFile: File): Single<File>

    fun createTmpThumbnail(videoFile: File): CompletableContinuation<File>
}